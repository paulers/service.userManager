using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Service.UserManager.Services;

public interface ICryptographyService
{
    string GenerateSalt(int byteLength = 128);
    string HashPassword(string password, string? customSalt = null);
    bool VerifyHashedPassword(string hashedPassword, string passwordToCompare, string? customSalt = null);
}

public class CryptographyService : ICryptographyService
{
    private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
    private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits
    private string _privateSalt;

    public CryptographyService(IConfiguration config)
    {
        _privateSalt = config["PrivateSalt"];
    }

    public string GenerateSalt(int byteLength = 128)
    {
        byte[] buf = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(buf);
    }

    public string HashPassword(string password, string? customSalt = null)
    {
        // Short circuit
        if (password == null) throw new ArgumentNullException("password");

        // Combine the password with a "server salt", a "custom salt" and the password itself
        // This is additional security, because even if the passwords/salts leak out of the database
        // the privateSalt is still missing
        string combinedPassword = $"{_privateSalt}{customSalt}{password}";

        // Prepare the byte arrays 
        using var deriveBytes = new Rfc2898DeriveBytes(combinedPassword, SaltSize, PBKDF2IterCount);
        byte[] salt = deriveBytes.Salt;
        byte[] subkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);

        // Create the hashed password byte array
        byte[] outputBytes = new byte[1 + SaltSize + PBKDF2SubkeyLength];

        // Copy the salt into the array
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);

        // Copy the subKey into the array
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, PBKDF2SubkeyLength);

        // Return the base64 encoded version
        return Convert.ToBase64String(outputBytes);
    }

    public bool VerifyHashedPassword(string hashedPassword, string passwordToCompare, string? customSalt = null)
    {
        // Short circuit
        if (hashedPassword == null) throw new ArgumentNullException("hashedPassword");
        if (passwordToCompare == null) throw new ArgumentNullException("password");

        // Get the hashed password into a byte array
        byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

        // Sanity check the length and header
        if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00) return false;

        // Extract the salt
        byte[] salt = new byte[SaltSize];
        Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);

        // Extract the sub key
        byte[] storedSubkey = new byte[PBKDF2SubkeyLength];
        Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

        // Turn the password we're comparing into a byte array, including the salt
        using var deriveBytes = new Rfc2898DeriveBytes(passwordToCompare, salt, PBKDF2IterCount);
        byte[] generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);

        // Return a comparison between the arrays
        return ByteArraysEqual(storedSubkey, generatedSubkey);
    }

    // This method is specifically written to not be optimized by the compiler since we're 
    // dealing with sensitive data
    [MethodImpl(MethodImplOptions.NoOptimization)]
    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        // Check reference pointers and short ciruit if same
        if (ReferenceEquals(a, b)) return true;
        // If either is null or aren't the same length, bail with error early
        if (a == null || b == null || a.Length != b.Length) return false;
        // Otherwise, we're comparing each element in the array
        bool areSame = true;
        for (int i = 0; i < a.Length; i++)
        {
            areSame &= (a[i] == b[i]);
        }
        return areSame;
    }
}

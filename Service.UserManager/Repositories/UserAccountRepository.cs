using Redis.OM;
using Redis.OM.Searching;
using Service.UserManager.Models;
using Service.UserManager.Services;
using Service.UserManager.ViewModel;

namespace Service.UserManager.Repositories;

public interface IUserAccountRepository
{
    Task<UserAccount> CreateUser(CreateUserViewModel model);
    Task<UserAccount?> UpdateUser(Guid id, UpdateUserViewModel model);
    Task<bool> DeleteUser(Guid id);
    Task<UserAccount?> GetUser(Guid id);
    Task<UserAccount?> GetUserByEmail(string email);
}

public class UserAccountRepository : IUserAccountRepository
{
    private RedisConnectionProvider _redis;
    private ICryptographyService _crypto;
    private IRedisCollection<UserAccount> _collection;

    public UserAccountRepository(RedisConnectionProvider redis, ICryptographyService cryptographyService)
    {
        _redis = redis;
        _crypto = cryptographyService;
        _collection = redis.RedisCollection<UserAccount>();

        redis.Connection.CreateIndex(typeof(UserAccount));
    }

    /// <summary>
    /// Creates the user in the database
    /// </summary>
    /// <param name="model">Model containing minimal information required to create a new user</param>
    /// <returns>Created user</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<UserAccount> CreateUser(CreateUserViewModel model)
    {
        // bail if password or email are empty
        if (string.IsNullOrEmpty(model.Password)) throw new ArgumentNullException(nameof(model.Password));
        if (string.IsNullOrEmpty(model.Email)) throw new ArgumentNullException(nameof(model.Email));

        // generate a salt and hash the password with it
        var salt = _crypto.GenerateSalt();
        var hashedPassword = _crypto.HashPassword(model.Password, salt);

        // instantiate the object we'll be saving to the database
        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow,
            Email = model.Email,
            Password = hashedPassword,
            PasswordSalt = salt,
            Confirmed = false,
            Enabled = true
        };

        await _collection.InsertAsync(user);

        return user;
    }

    /// <summary>
    /// Deletes the user from the database
    /// </summary>
    /// <param name="id">Id of the user to delete</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> DeleteUser(Guid id)
    {
        if (id == Guid.Empty) throw new Exception("Id cannot be null");
        var existingUser = await _collection.FindByIdAsync(id.ToString());
        if (existingUser == null) return false; // bail early if user not found
        await _collection.DeleteAsync(existingUser);
        return true;
    }

    /// <summary>
    /// Gets a user by their Id
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <returns>User object if found, null otherwise</returns>
    /// <exception cref="Exception"></exception>
    public async Task<UserAccount?> GetUser(Guid id)
    {
        if (id == Guid.Empty) throw new Exception("Id cannot be null");
        return await _collection.FindByIdAsync(id.ToString());
    }

    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>User object if found, null otherwise</returns>
    /// <exception cref="Exception"></exception>
    public async Task<UserAccount?> GetUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) throw new Exception("Email cannot be null");
        return await _collection.Where(t => t.Email == email).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates the user in the database
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<UserAccount?> UpdateUser(Guid id, UpdateUserViewModel model)
    {
        var existingUser = await _collection.FindByIdAsync(id.ToString());
        if (existingUser == null) return null;

        // Replace properties on the existing model, but only if they're being passed in
        existingUser.Name = model.Name ?? existingUser.Name;
        existingUser.Bio = model.Bio ?? existingUser.Bio;
        existingUser.Enabled = model.Enabled ?? existingUser.Enabled;
        existingUser.Confirmed = model.Confirmed ?? existingUser.Confirmed;
        existingUser.Metadata = model.Metadata ?? existingUser.Metadata;
        existingUser.LastLogin = model.LastLogin ?? existingUser.LastLogin;

        await _collection.UpdateAsync(existingUser);
        return existingUser;
    }
}

using Moq;
using Service.UserManager.Services;

namespace Service.UserManager.UnitTests
{
    public class CryptographyTests
    {
        [Fact]
        public void Verify_HashedPassword_WorksAsIntended()
        {
            var unhashed = "abc123";
            var hashed = "ACrMBIzHGveka+QeJYfkRU5s8KaX5yGPQYJXEnL/HEIO9ABjimnLLYUqC6yFhLj5Cg==";
            var salt = "Hcvbixc6ZptFqIGf53gqk7t+6nyqD0hGRqbu+OxTtrw+zga56YuSTPAGDee1DtmvolyS3EC4vlwGsFAPtoTfl0zwl7UvZk/OSAIGyZ9ZIAnmzcl0cVQ5kexqjx0RMh4D9gmI1zhXfv1RGvQF9f101xDUjAnB361Kzo88LDXnxSI=";

            var cryptoService = new Mock<ICryptographyService>();
        }
    }
}
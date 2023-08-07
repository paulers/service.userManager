using Service.UserManager.Models;

namespace Service.UserManager;

public static class Utilities
{
    public static void SanitizeUserAccountModel(UserAccount userAccount)
    {
        userAccount.Password = null;
        userAccount.PasswordSalt = null;
    }
}

using Service.UserManager.Models;

namespace Service.UserManager;

public static class Utilities
{
    public static void SanitizeUserAccountModel(UserAccount userAccount)
    {
        if (userAccount == null) return;
        userAccount.Password = null;
        userAccount.PasswordSalt = null;
    }
}

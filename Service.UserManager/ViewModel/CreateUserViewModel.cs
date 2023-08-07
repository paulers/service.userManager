using System.ComponentModel.DataAnnotations;

namespace Service.UserManager.ViewModel;

public record CreateUserViewModel
{
    [Required(AllowEmptyStrings = false)]
    public string? Email { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string? Password { get; set; }
}

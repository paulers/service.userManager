namespace Service.UserManager.ViewModel;

public record UpdateUserViewModel
{
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public bool? Enabled { get; set; }
    public bool? Confirmed { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime? LastLogin { get; set; }
}

using Redis.OM.Modeling;

namespace Service.UserManager.Models;

[Document(StorageType = StorageType.Json, IndexName = "users", Prefixes = new[] { "users" })]
public class UserAccount
{
    [Indexed][RedisIdField] public Guid? Id { get; set; }
    [Indexed] public DateTime? CreatedOn { get; set; }
    [Indexed] public DateTime? ModifiedOn { get; set; }
    [Indexed] public string? Email { get; set; }
    [Indexed] public string? Password { get; set; }
    [Indexed] public string? PasswordSalt { get; set; }
    [Searchable(Sortable = true)] public string? Name { get; set; }
    [Searchable] public string? Bio { get; set; }
    [Indexed] public Dictionary<string, object>? Metadata { get; set; }
    [Indexed] public bool? Enabled { get; set; }
    [Indexed] public bool? Confirmed { get; set; }

    [Indexed(Sortable = true)] public DateTime? LastLogin { get; set; }
    [Indexed] public int? LoginFailureCount { get; set; }
    [Indexed] public DateTime? LoginFailureOn { get; set; }
    [Indexed] public string? LoginFailureIpAddress { get; set; }
    [Indexed] public DateTime? LoginLockoutUntil { get; set; }
    [Indexed] public int? LoginFailureLockoutDuration { get; set; }
}

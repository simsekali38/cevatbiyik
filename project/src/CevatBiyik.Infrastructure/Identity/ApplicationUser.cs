using Microsoft.AspNetCore.Identity;

namespace CevatBiyik.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
}

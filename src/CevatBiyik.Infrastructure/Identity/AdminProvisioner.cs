using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.Infrastructure.Identity;

/// <summary>Explicit CLI operation only. Never seeds passwords or grants existing users a role.</summary>
public static class AdminProvisioner
{
    public static async Task ProvisionAsync(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles,
        IAuditLogService audit, IConfiguration configuration)
    {
        var name = configuration["AdminBootstrap:UserName"];
        var email = configuration["AdminBootstrap:Email"];
        var password = configuration["AdminBootstrap:Password"];
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Configure AdminBootstrap credentials via user-secrets or environment variables.");
        }

        var existing = await users.FindByNameAsync(name);
        if (existing is not null)
        {
            if (existing.IsActive && await users.IsInRoleAsync(existing, AdminAccess.Role))
            {
                return;
            }

            throw new InvalidOperationException("Existing account cannot be promoted by bootstrap.");
        }
        if (!await roles.RoleExistsAsync(AdminAccess.Role))
        {
            Ensure(await roles.CreateAsync(new IdentityRole(AdminAccess.Role)));
        }

        var user = new ApplicationUser { UserName = name, Email = email, EmailConfirmed = true };
        Ensure(await users.CreateAsync(user, password));
        Ensure(await users.AddToRoleAsync(user, AdminAccess.Role));
        await audit.WriteAsync(SecurityEvent.AdminProvisioned, user.Id);
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Admin provisioning failed; check configured credentials and password policy.");
        }
    }
}

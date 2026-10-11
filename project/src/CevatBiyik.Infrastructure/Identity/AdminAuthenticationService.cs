using System.Security.Claims;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CevatBiyik.Infrastructure.Identity;

public sealed class AdminAuthenticationService(UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn, IAuditLogService audit, IHttpContextAccessor accessor) : IAdminAuthenticationService
{
    private static readonly PasswordHasher<ApplicationUser> DummyHasher = new();
    private static readonly ApplicationUser DummyUser = new();
    private static readonly string DummyHash = DummyHasher.HashPassword(DummyUser, Guid.NewGuid().ToString("N"));
    public async Task<bool> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByNameAsync(userName);
        var allowed = false;
        if (user is not null)
        {
            var result = await signIn.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            allowed = result.Succeeded && user.IsActive && await users.IsInRoleAsync(user, AdminAccess.Role);
            allowed = allowed && !await users.GetTwoFactorEnabledAsync(user);
        }
        else
        {
            // Perform a password hash check for unknown names as well; reduce timing-based enumeration.
            DummyHasher.VerifyHashedPassword(DummyUser, DummyHash, password);
        }
        await audit.WriteAsync(allowed ? SecurityEvent.LoginSucceeded : SecurityEvent.LoginFailed,
            allowed ? user!.Id : null, cancellationToken);
        if (!allowed)
        {
            return false;
        }

        await signIn.SignInAsync(user!, isPersistent: false);
        return true;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        var actorId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await audit.WriteAsync(SecurityEvent.Logout, actorId, cancellationToken);
        await signIn.SignOutAsync();
    }
}

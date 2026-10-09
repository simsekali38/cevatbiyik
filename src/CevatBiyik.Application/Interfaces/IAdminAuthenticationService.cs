namespace CevatBiyik.Application.Interfaces;

public interface IAdminAuthenticationService
{
    Task<bool> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}

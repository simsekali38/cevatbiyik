namespace CevatBiyik.Application.Interfaces;

public interface ISlugService
{
    string Generate(string text);
    Task<string> GenerateUniqueAsync(string text, Func<string, CancellationToken, Task<bool>> exists,
        CancellationToken cancellationToken = default);
}

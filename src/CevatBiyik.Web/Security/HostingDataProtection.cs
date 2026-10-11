using Microsoft.AspNetCore.DataProtection;

namespace CevatBiyik.Web.Security;

public static class HostingDataProtection
{
    public static void AddHostingDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var path = configuration["DataProtection:KeysPath"];
        if (string.IsNullOrWhiteSpace(path)) { return; } // Existing development/test/IIS defaults remain available.
        if (!OperatingSystem.IsWindows()) { throw new InvalidOperationException("Configured hosting key storage requires Windows DPAPI."); }
        var web = Path.GetFullPath(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"));
        if (!Path.IsPathFullyQualified(path) || path.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("DataProtection:KeysPath must be an absolute private hosting directory.");
        }
        var keys = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);
        if (keys.Equals(web.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) ||
            keys.StartsWith(web.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Data protection keys cannot be stored in wwwroot.");
        }
        services.AddDataProtection().SetApplicationName("CevatBiyik.Web")
            .PersistKeysToFileSystem(new DirectoryInfo(keys)).ProtectKeysWithDpapi(protectToLocalMachine: true);
    }
}

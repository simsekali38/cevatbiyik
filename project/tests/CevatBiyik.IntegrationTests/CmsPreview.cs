using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
namespace CevatBiyik.IntegrationTests;
/// <summary>Optional local visual review of real Razor output with isolated test fixtures.</summary>
internal static class CmsPreview
{
    public static async Task ExportAsync(AdminWebFactory factory, HttpClient client, params (string Name, string Path)[] pages)
    {
        if (Environment.GetEnvironmentVariable("CEVATBIYIK_CMS_PREVIEW") != "1")
        {
            return;
        }

        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();
        var root = System.IO.Path.GetFullPath(System.IO.Path.Combine(environment.ContentRootPath, "../../artifacts/cms-preview"));
        Directory.CreateDirectory(root);
        foreach (var (name, path) in pages)
        {
            await File.WriteAllTextAsync(System.IO.Path.Combine(root, name + ".html"), await client.GetStringAsync(path));
        }

        foreach (var directory in new[] { "css", "js", "lib/bootstrap", "images/brand" })
        {
            var source = System.IO.Path.Combine(environment.WebRootPath, directory);
            foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                var target = System.IO.Path.Combine(root, directory, System.IO.Path.GetRelativePath(source, file));
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target)!); File.Copy(file, target, true);
            }
        }
        if (Directory.Exists(factory.UploadRoot))
        {
            foreach (var file in Directory.GetFiles(System.IO.Path.Combine(factory.UploadRoot, "images")))
            {
                var target = System.IO.Path.Combine(root, "uploads/images", System.IO.Path.GetFileName(file));
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target)!); File.Copy(file, target, true);
            }
        }
    }
}

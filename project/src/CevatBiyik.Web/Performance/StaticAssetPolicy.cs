using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;

namespace CevatBiyik.Web.Performance;

public static class StaticAssetPolicy
{
    public static void PrepareResponse(StaticFileResponseContext response)
    {
        var context = response.Context;
        var path = context.Request.Path;
        // MVC's content hash must match; an arbitrary ?v= value cannot make a mutable file immutable.
        var fingerprinted = false;
        if (context.Request.Query.Count == 1 && context.Request.Query.ContainsKey("v"))
        {
            var versions = context.RequestServices.GetRequiredService<IFileVersionProvider>();
            var versionedPath = versions.AddFileVersionToPath(context.Request.PathBase, path.Value!);
            fingerprinted = string.Equals(versionedPath, path + context.Request.QueryString.Value, StringComparison.Ordinal);
        }
        var immutableUpload = System.Text.RegularExpressions.Regex.IsMatch(path.Value!,
            @"\A/uploads/images/[a-f0-9]{32}\.webp\z", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        context.Response.Headers.CacheControl = fingerprinted || immutableUpload
            ? "public, max-age=31536000, immutable"
            : "public, max-age=3600, must-revalidate";
    }
}

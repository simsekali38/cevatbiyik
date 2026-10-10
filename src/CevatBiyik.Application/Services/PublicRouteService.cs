using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
namespace CevatBiyik.Application.Services;

public sealed class PublicRouteService(IArticleService articles, IPracticeAreaService areas, IProfileService profiles,
    IPageContentService pages, IFaqService faq, ILocationService locations) : IPublicRouteService
{
    public async Task<bool> ExistsAsync(string path, CancellationToken ct = default)
    {
        if (!RedirectPaths.IsSafe(path) || path != RedirectPaths.Normalize(path))
        {
            return false;
        }

        if (path is "/" or "/makaleler" or "/uzmanlik-alanlari" or "/iletisim")
        {
            return true;
        }

        if (path == "/konumlar")
        {
            return (await locations.IndexableAsync(ct)).Count > 0;
        }
        if (path.StartsWith("/konumlar/", StringComparison.Ordinal))
        {
            return await locations.PublishedAsync(path["/konumlar/".Length..], ct) is not null;
        }

        if (path == "/hakkimda")
        {
            return await profiles.GetAsync(true, ct) is not null;
        }

        if (path == "/sss")
        {
            return (await faq.ListAsync(publicOnly: true, ct: ct)).Items.Count > 0;
        }

        if (ContentPublication.Pages.ContainsKey(path))
        {
            return await pages.PublishedAsync(path, ct) is not null;
        }

        if (path.StartsWith("/makaleler/", StringComparison.Ordinal))
        {
            var slug = path["/makaleler/".Length..];
            return (await articles.GetPublishedBySlugAsync(slug, ct))?.Slug == slug;
        }
        if (path.StartsWith("/uzmanlik-alanlari/", StringComparison.Ordinal))
        {
            var slug = path["/uzmanlik-alanlari/".Length..];
            return (await areas.GetActiveBySlugAsync(slug, ct))?.Slug == slug;
        }
        return false;
    }
}

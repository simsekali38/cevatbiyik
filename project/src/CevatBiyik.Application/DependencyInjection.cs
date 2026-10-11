using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CevatBiyik.Application;

/// <summary>
/// Application katmanı servis kaydı. Use case'ler ve doğrulayıcılar ilgili fazlarda buraya eklenir.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ISlugService, SlugService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IPracticeAreaService, PracticeAreaService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IValidator<CreateArticleDto>, CreateArticleValidator>();
        services.AddScoped<IValidator<UpdateArticleDto>, UpdateArticleValidator>();
        services.AddScoped<IValidator<CreatePracticeAreaDto>, CreatePracticeAreaValidator>();
        services.AddScoped<IValidator<CreateContactMessageDto>, CreateContactMessageValidator>();
        services.AddScoped<IValidator<UpdateSiteSettingDto>, UpdateSiteSettingValidator>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<ITaxonomyService, TaxonomyService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IFileMaintenance, FileMaintenance>();
        services.AddScoped<IPageSeoService, PageSeoService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IValidator<LocationInput>, LocationValidator>();
        services.AddScoped<IRedirectService, RedirectService>();
        services.AddScoped<ISeoAuditService, SeoAuditService>();
        services.AddScoped<IPublicRouteService, PublicRouteService>();
        services.AddScoped<IValidator<RedirectInput>, RedirectValidator>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPageContentService, PageContentService>();
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IValidator<ProfileInput>, ProfileValidator>();
        services.AddScoped<IValidator<PageContentInput>, PageContentValidator>();
        services.AddScoped<IValidator<FaqInput>, FaqValidator>();
        return services;
    }
}

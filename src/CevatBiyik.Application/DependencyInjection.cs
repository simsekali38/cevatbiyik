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
        services.AddScoped<IPageSeoService, PageSeoService>();
        return services;
    }
}

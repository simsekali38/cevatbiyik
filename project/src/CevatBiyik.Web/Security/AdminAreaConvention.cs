using CevatBiyik.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace CevatBiyik.Web.Security;

public sealed class AdminAreaConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.RouteValues.TryGetValue("area", out var area) && area == "Admin")
        {
            controller.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().RequireRole(AdminAccess.Role).Build()));
        }
    }
}

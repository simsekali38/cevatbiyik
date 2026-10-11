using CevatBiyik.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CevatBiyik.Web.Security;

public sealed class PrivateDocumentsConvention(IConfiguration configuration) : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        if (configuration.GetValue<bool>("Features:PrivateDocuments")) { return; }
        foreach (var controller in application.Controllers.Where(x => x.ControllerType.AsType() == typeof(DocumentsController)).ToArray())
        {
            application.Controllers.Remove(controller);
        }
    }
}

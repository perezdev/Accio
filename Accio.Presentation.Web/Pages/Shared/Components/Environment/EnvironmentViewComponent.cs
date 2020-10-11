using Accio.Business.Services.ConfigurationServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Accio.Presentation.Web.Pages.Shared.Components.Environment
{
    public class EnvironmentViewComponent : ViewComponent
    {
        private ConfigurationService _configurationService { get; set; }

        public EnvironmentViewComponent(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var envType = _configurationService.GetEnvironment();
            return View(envType);
        }
    }
}

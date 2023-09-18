namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;

    using global::Nop.Services.Configuration;
    using global::Nop.Services.Localization;
    using global::Nop.Services.Messages;
    using global::Nop.Services.Security;
    using global::Nop.Web.Areas.Admin.Controllers;
    using global::Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
    using global::Nop.Web.Framework;
    using global::Nop.Web.Framework.Controllers;
    using global::Nop.Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class AutoTestingPluginController : BaseAdminController
    {
        private readonly ILocalizationService _localizationService;

        private readonly INotificationService _notificationService;

        private readonly IPermissionService _permissionService;

        private readonly ISettingService _settingService;

        private AutoTestingSettings _autoTestingSettings;

        public AutoTestingPluginController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            AutoTestingSettings autoTestingSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _autoTestingSettings = autoTestingSettings;
        }

        public virtual async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var model = _autoTestingSettings.ToSettingsModel<AutoTestingConfigureModel>();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Configure(AutoTestingConfigureModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            _autoTestingSettings = model.ToSettings(_autoTestingSettings);

            await _settingService.SaveSettingAsync(_autoTestingSettings);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("KSystem.Nop.Plugin.Core.SuccessSaveNotification"));

            return await Configure();
        }
    }
}

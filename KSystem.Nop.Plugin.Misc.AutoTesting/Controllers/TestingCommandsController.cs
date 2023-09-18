namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Core.Extensions;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Enums;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;

    using global::Nop.Services.Localization;
    using global::Nop.Services.Messages;
    using global::Nop.Services.Security;
    using global::Nop.Web.Areas.Admin.Controllers;
    using global::Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
    using global::Nop.Web.Framework;
    using global::Nop.Web.Framework.Models.Extensions;
    using global::Nop.Web.Framework.Mvc;
    using global::Nop.Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class TestingCommandsController : BaseAdminController
    {
        private readonly ILocalizationService _localizationService;

        private readonly INotificationService _notificationService;

        private readonly IPermissionService _permissionService;

        private readonly ITestingPageService _testingPageService;

        public TestingCommandsController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITestingPageService testingPageService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _testingPageService = testingPageService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridList(TestingCommandSearchModel searchModel, int testingPageId)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _testingPageService.GetAllTestingCommandsByPageIdAsync(
                testingPageId, searchModel.Page - 1, searchModel.PageSize);
            var allCommandTypes = await CommandType.AjaxComplete.ToSelectList();

            var model = new TestingCommandGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    var result = x.ToModel<TestingCommandModel>();

                    result.CommandTypeName = allCommandTypes.FirstOrDefault(y => y.Value == x.CommandTypeId.ToString())?.Text;

                    return result;
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateOrUpdate(int id, int testingPageId)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            var model = new TestingCommandModel
            {
                PageId = testingPageId
            };

            if (id > default(int))
            {
                var testingCommand = await _testingPageService.GetTestingCommandByIdAsync(id);

                if (testingCommand != null)
                {
                    model = testingCommand.ToModel<TestingCommandModel>();
                }
            }

            model.AvailableCommandTypes = await CommandType.AjaxComplete.ToSelectList();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateOrUpdate(TestingCommandModel model)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                ViewBag.RefreshPage = true;

                var entity = model.ToEntity<TestingCommand>();

                await _testingPageService.SaveTestingCommandEntryAsync(entity);

                return View(model);
            }

            model.AvailableCommandTypes = await CommandType.AjaxComplete.ToSelectList();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            await _testingPageService.DeleteTestingCommandEntryAsync(id);

            return new NullJsonResult();
        }
    }
}

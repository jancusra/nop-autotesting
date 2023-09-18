namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;

    using global::Nop.Services.Localization;
    using global::Nop.Services.Messages;
    using global::Nop.Services.Security;
    using global::Nop.Web.Areas.Admin.Controllers;
    using global::Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
    using global::Nop.Web.Framework;
    using global::Nop.Web.Framework.Controllers;
    using global::Nop.Web.Framework.Models.Extensions;
    using global::Nop.Web.Framework.Mvc;
    using global::Nop.Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class TestingPagesController : BaseAdminController
    {
        private readonly ILocalizationService _localizationService;

        private readonly INotificationService _notificationService;

        private readonly IPermissionService _permissionService;

        private readonly ITestingPageService _testingPageService;

        public TestingPagesController(
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var model = new TestingPageSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridList(TestingPageSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _testingPageService.GetAllTestingPagesAsync(searchModel.Page - 1, searchModel.PageSize);

            var model = new TestingPageGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    return x.ToModel<TestingPageModel>();
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var model = new TestingPageModel
            {
                IsModelCreate = true
            };

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TestingPageModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                var testingPage = model.ToEntity<TestingPage>();
                await _testingPageService.SaveTestingPageEntryAsync(testingPage);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("KSystem.Nop.Plugin.Core.Views.AddSuccess"));
                return continueEditing
                           ? RedirectToAction("Edit", new { id = testingPage.Id })
                           : RedirectToAction("List");
            }

            model.IsModelCreate = true;

            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var testingPage = await _testingPageService.GetTestingPageByIdAsync(id);
            if (testingPage == null)
            {
                return RedirectToAction("List");
            }

            var model = testingPage.ToModel<TestingPageModel>();
            model.CommandSearchModel.TestingPageId = id;

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(TestingPageModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var testingPage = await _testingPageService.GetTestingPageByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                testingPage = model.ToEntity(testingPage);
                await _testingPageService.SaveTestingPageEntryAsync(testingPage);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("KSystem.Nop.Plugin.Core.Views.UpdateSuccess"));

                if (continueEditing)
                {
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = testingPage.Id });
                }

                return RedirectToAction("List");
            }

            model.CommandSearchModel.TestingPageId = testingPage.Id;

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("copy-test-page")]
        public virtual async Task<IActionResult> CopyTestingPage(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var testingPage = await _testingPageService.GetTestingPageByIdAsync(id);
            var testingCommands = await _testingPageService.GetAllTestingCommandsByPageIdAsync(id);

            testingPage.Id = default(int);
            testingPage.Name += " - Copy";

            await _testingPageService.SaveTestingPageEntryAsync(testingPage);

            foreach (var testingCommand in testingCommands)
            {
                testingCommand.Id = default(int);
                testingCommand.PageId = testingPage.Id;

                await _testingPageService.SaveTestingCommandEntryAsync(testingCommand);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            await _testingPageService.DeleteTestingPageEntryAsync(id);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteTestingPage(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return await AccessDeniedDataTablesJson();
            }

            await _testingPageService.DeleteTestingPageEntryAsync(id);

            return new NullJsonResult();
        }
    }
}

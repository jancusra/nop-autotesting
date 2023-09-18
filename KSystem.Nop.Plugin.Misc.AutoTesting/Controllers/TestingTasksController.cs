namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Shared;

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
    public partial class TestingTasksController : BaseAdminController
    {
        private readonly ILocalizationService _localizationService;

        private readonly INotificationService _notificationService;

        private readonly IPermissionService _permissionService;

        private readonly ITaskReportService _taskReportService;

        private readonly ITestingPageService _testingPageService;

        private readonly ITestingTaskService _testingTaskService;

        public TestingTasksController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITaskReportService taskReportService,
            ITestingPageService testingPageService,
            ITestingTaskService testingTaskService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _taskReportService = taskReportService;
            _testingPageService = testingPageService;
            _testingTaskService = testingTaskService;
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var model = new TestingTaskSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridList(TestingTaskSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _testingTaskService.GetAllTestingTasksAsync(searchModel.Page - 1, searchModel.PageSize);

            var model = new TestingTaskGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    return x.ToModel<TestingTaskModel>();
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

            var model = new TestingTaskModel
            {
                IsModelCreate = true
            };

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TestingTaskModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                var testingTask = model.ToEntity<TestingTask>();
                await _testingTaskService.SaveTestingTaskEntryAsync(testingTask);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("KSystem.Nop.Plugin.Core.Views.AddSuccess"));
                return continueEditing
                           ? RedirectToAction("Edit", new { id = testingTask.Id })
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

            var testingTask = await _testingTaskService.GetTestingTaskByIdAsync(id);
            if (testingTask == null)
            {
                return RedirectToAction("List");
            }

            var model = testingTask.ToModel<TestingTaskModel>();
            model.TaskPageSearchModel.TestingTaskId = id;

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(TestingTaskModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var testingTask = await _testingTaskService.GetTestingTaskByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                testingTask = model.ToEntity(testingTask);
                await _testingTaskService.SaveTestingTaskEntryAsync(testingTask);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("KSystem.Nop.Plugin.Core.Views.UpdateSuccess"));

                if (continueEditing)
                {
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = testingTask.Id });
                }

                return RedirectToAction("List");
            }

            model.TaskPageSearchModel.TestingTaskId = testingTask.Id;

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            await _testingTaskService.DeleteTestingTaskEntryAsync(id);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteTestingTask(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return await AccessDeniedDataTablesJson();
            }

            await _testingTaskService.DeleteTestingTaskEntryAsync(id);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RunTask(int id)
        {
            var testingTaskPageMap = (await _testingTaskService.GetAllActiveTestingPagesByTaskIdAsync(id)).FirstOrDefault();

            if (testingTaskPageMap != null)
            {
                await _taskReportService.SaveExecutedTaskEntryAsync(new ExecutedTask {
                    TaskId = testingTaskPageMap.TaskId,
                    LastRun = DateTime.Now
                });

                var testingUrl = await _testingPageService.GetTestingUrlByPageIdAsync(testingTaskPageMap.PageId);
                var parameterDelimeter = _testingPageService.GetTestingUrlParameterDelimeter(testingUrl);

                return Redirect($"{testingUrl}{parameterDelimeter}{AutoTestingDefaults.TestingTaskPageUrlParameterName}={testingTaskPageMap.Id}");
            }

            return RedirectToAction("List");
        }
    }
}

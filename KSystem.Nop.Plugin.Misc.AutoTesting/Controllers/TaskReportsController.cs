namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TaskReports;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;

    using global::Nop.Services.Security;
    using global::Nop.Web.Areas.Admin.Controllers;
    using global::Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
    using global::Nop.Web.Framework;
    using global::Nop.Web.Framework.Models.Extensions;
    using global::Nop.Web.Framework.Mvc;
    using global::Nop.Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class TaskReportsController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;

        private readonly ITestingTaskService _testingTaskService;

        private readonly ITaskReportService _taskReportService;

        public TaskReportsController(
            IPermissionService permissionService,
            ITestingTaskService testingTaskService,
            ITaskReportService taskReportService)
        {
            _permissionService = permissionService;
            _testingTaskService = testingTaskService;
            _taskReportService = taskReportService;
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            var model = new ExecutedTaskSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridList(ExecutedTaskSearchModel searchModel)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _taskReportService.GetAllExecutedTasksAsync(searchModel.Page - 1, searchModel.PageSize);
            var testingTasks = await _testingTaskService.GetAllTestingTasksAsync();

            var model = new ExecutedTaskGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    var result = x.ToModel<ExecutedTaskModel>();

                    result.TaskName = testingTasks.FirstOrDefault(y => y.Id == x.TaskId)?.Name;

                    return result;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridListOfMessages(ReportedMessageSearchModel searchModel)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _taskReportService.GetAllReportedMessagesByExecutedTaskIdAsync(
                searchModel.ExecutedTaskId, searchModel.Page - 1, searchModel.PageSize);

            var model = new ReportedMessageGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    return x.ToModel<ReportedMessageModel>();
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> Detail(int id)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            var executedTask = await _taskReportService.GetExecutedTaskByIdAsync(id);
            if (executedTask == null)
            {
                return RedirectToAction("List");
            }

            var model = executedTask.ToModel<ExecutedTaskModel>();
            model.MessageSearchModel.ExecutedTaskId = id;

            var testingTask = await _testingTaskService.GetTestingTaskByIdAsync(executedTask.TaskId);
            if (testingTask != null)
            {
                model.TaskName = testingTask.Name;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            await _taskReportService.DeleteExecutedTaskEntryAsync(id);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteExecutedTask(int id)
        {
            if (!await CanManagePluginsAsync(_permissionService))
            {
                return AccessDeniedView();
            }

            await _taskReportService.DeleteExecutedTaskEntryAsync(id);

            return new NullJsonResult();
        }
    }
}

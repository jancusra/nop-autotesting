namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks;
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
    public partial class TestingTaskPagesController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;

        private readonly ITestingPageService _testingPageService;

        private readonly ITestingTaskService _testingTaskService;

        public TestingTaskPagesController(
            IPermissionService permissionService,
            ITestingPageService testingPageService,
            ITestingTaskService testingTaskService)
        {
            _permissionService = permissionService;
            _testingPageService = testingPageService;
            _testingTaskService = testingTaskService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> GridList(TestingTaskPageSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return await AccessDeniedDataTablesJson();
            }

            var records = await _testingTaskService.GetAllTestingPagesByTaskIdAsync(
                searchModel.TestingTaskId, searchModel.Page - 1, searchModel.PageSize);
            var allTestingPages = await _testingPageService.GetAllTestingPagesAsync();

            var model = new TestingTaskPageGridModel().PrepareToGrid(searchModel, records, () =>
            {
                return records.Select(x =>
                {
                    var result = x.ToModel<TestingTaskPageModel>();

                    result.PageName = allTestingPages.FirstOrDefault(y => y.Id == x.PageId)?.Name;

                    return result;
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> AddOrUpdate(int id, int testingTaskId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            var model = new TestingTaskPageModel
            {
                TaskId = testingTaskId
            };

            if (id > default(int))
            {
                var testingTaskPage = await _testingTaskService.GetTestingTaskPageMapByIdAsync(id);

                if (testingTaskPage != null)
                {
                    model = testingTaskPage.ToModel<TestingTaskPageModel>();
                }
            }

            model.AvailablePages = await GetPagesSelectListAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddOrUpdate(TestingTaskPageModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                ViewBag.RefreshPage = true;

                var entity = model.ToEntity<TestingTaskPageMap>();

                await _testingTaskService.SaveTestingTaskPageMapEntryAsync(entity);

                return View(model);
            }

            model.AvailablePages = await GetPagesSelectListAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return AccessDeniedView();
            }

            await _testingTaskService.DeleteTestingTaskPageEntryAsync(id);

            return new NullJsonResult();
        }

        private async Task<IList<SelectListItem>> GetPagesSelectListAsync()
        {
            return await (await _testingPageService.GetAllTestingPagesAsync()).Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToListAsync();
        }
    }
}

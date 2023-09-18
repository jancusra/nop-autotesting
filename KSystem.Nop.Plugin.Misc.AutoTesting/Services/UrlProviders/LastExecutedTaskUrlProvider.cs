namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;

    using global::Nop.Core;
    using global::Nop.Services.Seo;

    public class LastExecutedTaskUrlProvider : BaseTestingUrlProvider, ILastExecutedTaskUrlProvider
    {
        private readonly ITaskReportService _taskReportService;

        public LastExecutedTaskUrlProvider(
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ITaskReportService taskReportService)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _taskReportService = taskReportService;
        }

        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var executedTask = await _taskReportService.GetLastExecutedTaskAsync();

            if (executedTask != null)
            {
                return GetFinalUrlBySeName($"Admin/TaskReports/Detail/{executedTask.Id}");
            }

            return string.Empty;
        }
    }
}

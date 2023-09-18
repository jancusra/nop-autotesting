namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TaskReports
{
    using global::Nop.Web.Framework.Models;

    public record ReportedMessageSearchModel : BaseSearchModel
    {
        public int ExecutedTaskId { get; set; }
    }
}
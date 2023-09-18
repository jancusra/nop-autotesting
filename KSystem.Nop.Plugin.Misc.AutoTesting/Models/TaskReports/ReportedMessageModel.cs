namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TaskReports
{
    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record ReportedMessageModel : BaseNopEntityModel
    {
        public int ExecutedTaskId { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.PageName")]
        public string PageName { get; set; }

        [NopResourceDisplayName("Admin.System.Log.List.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Success")]
        public bool Success { get; set; }
    }
}
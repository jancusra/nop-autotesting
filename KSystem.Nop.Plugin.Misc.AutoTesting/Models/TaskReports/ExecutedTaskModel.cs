namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TaskReports
{
    using System;

    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record ExecutedTaskModel : BaseNopEntityModel
    {
        public ExecutedTaskModel()
        {
            MessageSearchModel = new ReportedMessageSearchModel();
            MessageSearchModel.SetGridPageSize();
        }

        public int TaskId { get; set; }

        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        public string TaskName { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Core.Fields.LastRun")]
        public DateTime LastRun { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Core.Fields.LastFinish")]
        public DateTime? LastFinish { get; set; }

        public ReportedMessageSearchModel MessageSearchModel { get; set; }
    }
}
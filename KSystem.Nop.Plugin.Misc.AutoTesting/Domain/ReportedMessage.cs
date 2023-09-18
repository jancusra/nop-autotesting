namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    public class ReportedMessage : BaseEntity
    {
        public int ExecutedTaskId { get; set; }

        public string PageName { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}

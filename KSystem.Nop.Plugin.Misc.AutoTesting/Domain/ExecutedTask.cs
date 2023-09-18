namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using System;

    using global::Nop.Core;

    public class ExecutedTask : BaseEntity
    {
        public int TaskId { get; set; }

        public DateTime LastRun { get; set; }

        public DateTime? LastFinish { get; set; }
    }
}

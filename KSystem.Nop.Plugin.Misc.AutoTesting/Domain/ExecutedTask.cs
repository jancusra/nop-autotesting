namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using System;

    using global::Nop.Core;

    /// <summary>
    /// Represents an executed task
    /// </summary>
    public class ExecutedTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the testing task identifier
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets date and time of last run
        /// </summary>
        public DateTime LastRun { get; set; }

        /// <summary>
        /// Gets or sets date and time of last finish
        /// </summary>
        public DateTime? LastFinish { get; set; }
    }
}

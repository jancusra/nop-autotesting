namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    /// <summary>
    /// Represents a report message for an executed task
    /// </summary>
    public class ReportedMessage : BaseEntity
    {
        /// <summary>
        /// Gets or sets the executed task identifier
        /// </summary>
        public int ExecutedTaskId { get; set; }

        /// <summary>
        /// Gets or sets the page name
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the success state
        /// </summary>
        public bool Success { get; set; }
    }
}

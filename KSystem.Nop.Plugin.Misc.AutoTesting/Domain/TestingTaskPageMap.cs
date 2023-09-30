namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    /// <summary>
    /// Represents a testing task mapped to testing page
    /// </summary>
    public class TestingTaskPageMap : BaseEntity
    {
        /// <summary>
        /// Gets or sets the testing task identifier
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the testing page identifier
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Gets or sets the page order
        /// </summary>
        public int PageOrder { get; set; }

        /// <summary>
        /// Gets or sets the inclusion in the task
        /// </summary>
        public bool IncludedInTask { get; set; }
    }
}

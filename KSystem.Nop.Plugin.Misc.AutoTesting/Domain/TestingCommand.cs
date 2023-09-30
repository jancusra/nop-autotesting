namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    /// <summary>
    /// Represents a testing command
    /// </summary>
    public class TestingCommand : BaseEntity
    {
        /// <summary>
        /// Gets or sets the testing page identifier
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Gets or sets the testing command type identifier
        /// </summary>
        public int CommandTypeId { get; set; }

        /// <summary>
        /// Gets or sets the selector
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the report message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the command order
        /// </summary>
        public int CommandOrder { get; set; }
    }
}

namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    /// <summary>
    /// Represents a testing page
    /// </summary>
    public class TestingPage : BaseEntity
    {
        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the testing URL
        /// </summary>
        public string TestingUrl { get; set; }

        /// <summary>
        /// Gets or sets the custom URL provider method name
        /// </summary>
        public string CustomUrlProvider { get; set; }

        /// <summary>
        /// Gets or sets the provider parameters
        /// </summary>
        public string ProviderParameters { get; set; }
    }
}

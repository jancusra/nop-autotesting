namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    public class TestingPage : BaseEntity
    {
        public string Name { get; set; }

        public string TestingUrl { get; set; }

        public string CustomUrlProvider { get; set; }

        public string ProviderParameters { get; set; }
    }
}

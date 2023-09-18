namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages
{
    using global::Nop.Web.Framework.Models;

    public record TestingCommandSearchModel : BaseSearchModel
    {
        public int TestingPageId { get; set; }
    }
}
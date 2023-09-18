namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages
{
    using System.ComponentModel.DataAnnotations;

    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record TestingPageModel : BaseNopEntityModel
    {
        public TestingPageModel()
        {
            CommandSearchModel = new TestingCommandSearchModel();
            CommandSearchModel.SetGridPageSize();
        }

        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        [Required]
        public string Name { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.TestingUrl")]
        public string TestingUrl { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CustomUrlProvider")]
        public string CustomUrlProvider { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.ProviderParametersWithFormat")]
        public string ProviderParameters { get; set; }

        public TestingCommandSearchModel CommandSearchModel { get; set; }

        public bool IsModelCreate { get; set; }
    }
}

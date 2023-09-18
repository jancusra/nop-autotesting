namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record TestingCommandModel : BaseNopEntityModel
    {
        public TestingCommandModel()
        {
            AvailableCommandTypes = new List<SelectListItem>();
        }

        public int PageId { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CommandType")]
        public int CommandTypeId { get; set; }
        public string CommandTypeName { get; set; }
        public IList<SelectListItem> AvailableCommandTypes { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Selector")]
        public string Selector { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Value")]
        public string Value { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CommandOrder")]
        public int CommandOrder { get; set; }
    }
}
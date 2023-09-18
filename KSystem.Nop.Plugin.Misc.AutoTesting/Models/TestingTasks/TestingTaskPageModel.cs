namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record TestingTaskPageModel : BaseNopEntityModel
    {
        public TestingTaskPageModel()
        {
            AvailablePages = new List<SelectListItem>();
        }

        public int TaskId { get; set; }

        public int PageId { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.PageName")]
        public string PageName { get; set; }
        public IList<SelectListItem> AvailablePages { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.PageOrder")]
        public int PageOrder { get; set; }

        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.IncludedInTask")]
        public bool IncludedInTask { get; set; }
    }
}

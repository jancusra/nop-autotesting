namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks
{
    using System.ComponentModel.DataAnnotations;

    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record TestingTaskModel : BaseNopEntityModel
    {
        public TestingTaskModel()
        {
            TaskPageSearchModel = new TestingTaskPageSearchModel();
            TaskPageSearchModel.SetGridPageSize();
        }

        [NopResourceDisplayName("Admin.System.SeNames.Name")]
        [Required]
        public string Name { get; set; }

        public TestingTaskPageSearchModel TaskPageSearchModel { get; set; }

        public bool IsModelCreate { get; set; }
    }
}

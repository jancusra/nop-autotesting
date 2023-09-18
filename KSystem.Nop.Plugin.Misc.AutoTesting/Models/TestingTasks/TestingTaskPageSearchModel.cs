namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks
{
    using global::Nop.Web.Framework.Models;

    public record TestingTaskPageSearchModel : BaseSearchModel
    {
        public int TestingTaskId { get; set; }
    }
}
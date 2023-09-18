namespace KSystem.Nop.Plugin.Misc.AutoTesting.Factories
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;

    public interface IAutoTestingFactory
    {
        Task<TestingWidgetModel> PrepareTestingWidgetModelAsync(int testingTaskPageMapId);

        Task<TestingWidgetModel> ValidateTaskPageAndPrepareTestingWidgetModelAsync(int testingTaskPageMapId, string actualPath);
    }
}

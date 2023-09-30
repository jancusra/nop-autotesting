namespace KSystem.Nop.Plugin.Misc.AutoTesting.Factories
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;

    /// <summary>
    /// Provides plugin view models preparations and validation
    /// </summary>
    public interface IAutoTestingFactory
    {
        /// <summary>
        /// Prepare injected testing widget model
        /// </summary>
        /// <param name="testingTaskPageMapId">Testing task-page map identifier</param>
        /// <returns>Prepared testing widget model</returns>
        Task<TestingWidgetModel> PrepareTestingWidgetModelAsync(int testingTaskPageMapId);

        /// <summary>
        /// Validate task page and return widget model if OK
        /// </summary>
        /// <param name="testingTaskPageMapId">Testing task-page map identifier</param>
        /// <param name="actualPath">actual testing URL path</param>
        /// <returns>Prepared testing widget model</returns>
        Task<TestingWidgetModel> ValidateTaskPageAndPrepareTestingWidgetModelAsync(int testingTaskPageMapId, string actualPath);
    }
}

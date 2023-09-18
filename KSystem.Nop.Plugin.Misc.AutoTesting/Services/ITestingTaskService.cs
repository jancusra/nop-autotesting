namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    public interface ITestingTaskService
    {
        Task<TestingTask> GetTestingTaskByIdAsync(int testingTaskId);
        Task<IPagedList<TestingTask>> GetAllTestingTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        Task SaveTestingTaskEntryAsync(TestingTask testingTaskEntry);
        Task DeleteTestingTaskEntryAsync(int testingTaskEntryId);

        Task<TestingTaskPageMap> GetTestingTaskPageMapByIdAsync(int testingTaskPageId);
        Task<TestingTaskPageMap> GetNextTestingTaskPageMapByMapAsync(TestingTaskPageMap testingTaskPageMap);
        Task<List<TestingTaskPageMap>> GetAllActiveTestingPagesByTaskIdAsync(int testingTaskId);
        Task<IPagedList<TestingTaskPageMap>> GetAllTestingPagesByTaskIdAsync(
            int testingTaskId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);
        Task SaveTestingTaskPageMapEntryAsync(TestingTaskPageMap testingTaskPageMap);
        Task DeleteTestingTaskPageEntryAsync(int testingTaskPageMapEntryId);
    }
}

namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    /// <summary>
    /// Testing task service - manage testing tasks and mapped pages
    /// </summary>
    public interface ITestingTaskService
    {
        /// <summary>
        /// Get testing task by identifier
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <returns>testing task</returns>
        Task<TestingTask> GetTestingTaskByIdAsync(int testingTaskId);

        /// <summary>
        /// Get all testing tasks for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing tasks</returns>
        Task<IPagedList<TestingTask>> GetAllTestingTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update testing task entry
        /// </summary>
        /// <param name="testingTaskEntry">testing task entity</param>
        Task SaveTestingTaskEntryAsync(TestingTask testingTaskEntry);

        /// <summary>
        /// Delete testing task by identifier
        /// </summary>
        /// <param name="testingTaskEntryId">testing task identifier</param>
        Task DeleteTestingTaskEntryAsync(int testingTaskEntryId);

        /// <summary>
        /// Get testing task page map by identifier
        /// </summary>
        /// <param name="testingTaskPageId">testing task page map identifier</param>
        /// <returns>testing task page map</returns>
        Task<TestingTaskPageMap> GetTestingTaskPageMapByIdAsync(int testingTaskPageId);

        /// <summary>
        /// Get the next testing task page map by actual task page map
        /// </summary>
        /// <param name="testingTaskPageMap">testing task page map entity</param>
        /// <returns>testing task page map</returns>
        Task<TestingTaskPageMap> GetNextTestingTaskPageMapByMapAsync(TestingTaskPageMap testingTaskPageMap);

        /// <summary>
        /// Get all active testing task page maps by task identifier
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <returns>list of active testing task page maps</returns>
        Task<List<TestingTaskPageMap>> GetAllActiveTestingPagesByTaskIdAsync(int testingTaskId);

        /// <summary>
        /// Get all testing pages by task identifier for a table
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing task page maps</returns>
        Task<IPagedList<TestingTaskPageMap>> GetAllTestingPagesByTaskIdAsync(
            int testingTaskId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update testing task page map entry
        /// </summary>
        /// <param name="testingTaskPageMap">testing task page map entity</param>
        Task SaveTestingTaskPageMapEntryAsync(TestingTaskPageMap testingTaskPageMap);

        /// <summary>
        /// Delete testing task page map by identifier
        /// </summary>
        /// <param name="testingTaskPageMapEntryId">testing task page map identifier</param>
        Task DeleteTestingTaskPageMapEntryAsync(int testingTaskPageMapEntryId);
    }
}

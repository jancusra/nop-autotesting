namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;
    using global::Nop.Data;

    /// <summary>
    /// Testing task service - manage testing tasks and mapped pages
    /// </summary>
    public class TestingTaskService : ITestingTaskService
    {
        private readonly IRepository<TestingTask> _testingTaskRepository;

        private readonly IRepository<TestingTaskPageMap> _testingTaskPageMapRepository;

        public TestingTaskService(
            IRepository<TestingTask> testingTaskRepository,
            IRepository<TestingTaskPageMap> testingTaskPageMapRepository)
        {
            _testingTaskRepository = testingTaskRepository;
            _testingTaskPageMapRepository = testingTaskPageMapRepository;
        }

        #region Testing tasks

        /// <summary>
        /// Get testing task by identifier
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <returns>testing task</returns>
        public virtual async Task<TestingTask> GetTestingTaskByIdAsync(int testingTaskId)
        {
            return await _testingTaskRepository.Table.FirstOrDefaultAsync(x => x.Id == testingTaskId);
        }

        /// <summary>
        /// Get all testing tasks for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing tasks</returns>
        public virtual async Task<IPagedList<TestingTask>> GetAllTestingTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<TestingTask>(await _testingTaskRepository.Table.ToListAsync(), pageIndex, pageSize);
        }

        /// <summary>
        /// Add new or update testing task entry
        /// </summary>
        /// <param name="testingTaskEntry">testing task entity</param>
        public virtual async Task SaveTestingTaskEntryAsync(TestingTask testingTaskEntry)
        {
            if (testingTaskEntry.Id == default(int))
            {
                await _testingTaskRepository.InsertAsync(testingTaskEntry);
            }
            else
            {
                await _testingTaskRepository.UpdateAsync(testingTaskEntry);
            }
        }

        /// <summary>
        /// Delete testing task by identifier
        /// </summary>
        /// <param name="testingTaskEntryId">testing task identifier</param>
        public virtual async Task DeleteTestingTaskEntryAsync(int testingTaskEntryId)
        {
            var testingTaskEntry = await GetTestingTaskByIdAsync(testingTaskEntryId);

            if (testingTaskEntry != null)
            {
                await _testingTaskRepository.DeleteAsync(testingTaskEntry);
            }
        }

        #endregion

        #region Testing task - page maps

        /// <summary>
        /// Get testing task page map by identifier
        /// </summary>
        /// <param name="testingTaskPageId">testing task page map identifier</param>
        /// <returns>testing task page map</returns>
        public virtual async Task<TestingTaskPageMap> GetTestingTaskPageMapByIdAsync(int testingTaskPageId)
        {
            return await _testingTaskPageMapRepository.Table.FirstOrDefaultAsync(x => x.Id == testingTaskPageId);
        }

        /// <summary>
        /// Get the next testing task page map by actual task page map
        /// </summary>
        /// <param name="testingTaskPageMap">testing task page map entity</param>
        /// <returns>testing task page map</returns>
        public virtual async Task<TestingTaskPageMap> GetNextTestingTaskPageMapByMapAsync(TestingTaskPageMap testingTaskPageMap)
        {
            var allTestingPagesByTask = await GetAllActiveTestingPagesByTaskIdAsync(testingTaskPageMap.TaskId);
            int listItemIndex = allTestingPagesByTask.FindIndex(x => x.Id == testingTaskPageMap.Id);

            if (listItemIndex + 1 < allTestingPagesByTask.Count)
            {
                return allTestingPagesByTask.ElementAt(listItemIndex + 1);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get all active testing task page maps by task identifier
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <returns>list of active testing task page maps</returns>
        public virtual async Task<List<TestingTaskPageMap>> GetAllActiveTestingPagesByTaskIdAsync(int testingTaskId)
        {
            return await (await GetAllTestingPagesByTaskIdAsync(testingTaskId)).Where(x => x.IncludedInTask).ToListAsync();
        }

        /// <summary>
        /// Get all testing pages by task identifier for a table
        /// </summary>
        /// <param name="testingTaskId">testing task identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing task page maps</returns>
        public virtual async Task<IPagedList<TestingTaskPageMap>> GetAllTestingPagesByTaskIdAsync(
            int testingTaskId, 
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            return new PagedList<TestingTaskPageMap>(
                await _testingTaskPageMapRepository.Table
                    .Where(x => x.TaskId == testingTaskId)
                    .OrderBy(y => y.PageOrder)
                    .ToListAsync(), pageIndex, pageSize);
        }

        /// <summary>
        /// Add new or update testing task page map entry
        /// </summary>
        /// <param name="testingTaskPageMap">testing task page map entity</param>
        public virtual async Task SaveTestingTaskPageMapEntryAsync(TestingTaskPageMap testingTaskPageMap)
        {
            if (testingTaskPageMap.Id == default(int))
            {
                await _testingTaskPageMapRepository.InsertAsync(testingTaskPageMap);
            }
            else
            {
                await _testingTaskPageMapRepository.UpdateAsync(testingTaskPageMap);
            }
        }

        /// <summary>
        /// Delete testing task page map by identifier
        /// </summary>
        /// <param name="testingTaskPageMapEntryId">testing task page map identifier</param>
        public virtual async Task DeleteTestingTaskPageMapEntryAsync(int testingTaskPageMapEntryId)
        {
            var testingTaskPageMapEntry = await GetTestingTaskPageMapByIdAsync(testingTaskPageMapEntryId);

            if (testingTaskPageMapEntry != null)
            {
                await _testingTaskPageMapRepository.DeleteAsync(testingTaskPageMapEntry);
            }
        }

        #endregion
    }
}

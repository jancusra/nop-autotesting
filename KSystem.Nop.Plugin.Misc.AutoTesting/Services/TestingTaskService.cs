namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;
    using global::Nop.Data;

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

        public virtual async Task<TestingTask> GetTestingTaskByIdAsync(int testingTaskId)
        {
            return await _testingTaskRepository.Table.FirstOrDefaultAsync(x => x.Id == testingTaskId);
        }

        public virtual async Task<IPagedList<TestingTask>> GetAllTestingTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<TestingTask>(await _testingTaskRepository.Table.ToListAsync(), pageIndex, pageSize);
        }

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

        public virtual async Task<TestingTaskPageMap> GetTestingTaskPageMapByIdAsync(int testingTaskPageId)
        {
            return await _testingTaskPageMapRepository.Table.FirstOrDefaultAsync(x => x.Id == testingTaskPageId);
        }

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

        public virtual async Task<List<TestingTaskPageMap>> GetAllActiveTestingPagesByTaskIdAsync(int testingTaskId)
        {
            return await (await GetAllTestingPagesByTaskIdAsync(testingTaskId)).Where(x => x.IncludedInTask).ToListAsync();
        }

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

        public virtual async Task DeleteTestingTaskPageEntryAsync(int testingTaskPageMapEntryId)
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

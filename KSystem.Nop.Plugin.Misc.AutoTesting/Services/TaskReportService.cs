namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data;
    using global::Nop.Core;

    public class TaskReportService : ITaskReportService
    {
        private readonly IRepository<ExecutedTask> _executedTaskRepository;

        private readonly IRepository<ReportedMessage> _reportedMessageRepository;

        public TaskReportService(
            IRepository<ExecutedTask> executedTaskRepository,
            IRepository<ReportedMessage> reportedMessageRepository)
        {
            _executedTaskRepository = executedTaskRepository;
            _reportedMessageRepository = reportedMessageRepository;
        }

        #region Executed tasks

        public virtual async Task<ExecutedTask> GetExecutedTaskByIdAsync(int executeTaskId)
        {
            return await _executedTaskRepository.Table.FirstOrDefaultAsync(x => x.Id == executeTaskId);
        }

        public virtual async Task<ExecutedTask> GetLastExecutedTaskAsync()
        {
            return await _executedTaskRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public virtual async Task<IPagedList<ExecutedTask>> GetAllExecutedTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<ExecutedTask>(
                await _executedTaskRepository.Table
                .OrderByDescending(x => x.Id)
                .ToListAsync(), pageIndex, pageSize);
        }

        public virtual async Task SaveExecutedTaskEntryAsync(ExecutedTask executedTaskEntry)
        {
            if (executedTaskEntry.Id == default(int))
            {
                await _executedTaskRepository.InsertAsync(executedTaskEntry);
            }
            else
            {
                await _executedTaskRepository.UpdateAsync(executedTaskEntry);
            }
        }

        public virtual async Task DeleteExecutedTaskEntryAsync(int executeTaskId)
        {
            var executedTaskEntry = await GetExecutedTaskByIdAsync(executeTaskId);

            if (executedTaskEntry != null)
            {
                await _executedTaskRepository.DeleteAsync(executedTaskEntry);
            }
        }

        #endregion

        #region Reported messages

        public virtual async Task<ReportedMessage> GetReportedMessageByIdAsync(int reportedMessageId)
        {
            return await _reportedMessageRepository.Table.FirstOrDefaultAsync(x => x.Id == reportedMessageId);
        }

        public virtual async Task<IPagedList<ReportedMessage>> GetAllReportedMessagesByExecutedTaskIdAsync(
            int executeTaskId,
            int pageIndex = 0, 
            int pageSize = int.MaxValue)
        {
            return new PagedList<ReportedMessage>(
                await _reportedMessageRepository.Table
                .Where(x => x.ExecutedTaskId == executeTaskId)
                .OrderBy(x => x.Id)
                .ToListAsync(), pageIndex, pageSize);
        }

        public virtual async Task SaveReportedMessageEntryAsync(ReportedMessage reportedMessageEntry)
        {
            if (reportedMessageEntry.Id == default(int))
            {
                await _reportedMessageRepository.InsertAsync(reportedMessageEntry);
            }
            else
            {
                await _reportedMessageRepository.UpdateAsync(reportedMessageEntry);
            }
        }

        #endregion
    }
}

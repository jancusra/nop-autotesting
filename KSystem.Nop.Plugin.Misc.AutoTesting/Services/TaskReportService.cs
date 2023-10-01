namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data;
    using global::Nop.Core;

    /// <summary>
    /// Task report service - manage executed tasks and reports
    /// </summary>
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

        /// <summary>
        /// Get excuted task by identifier
        /// </summary>
        /// <param name="executeTaskId">executed task identifier</param>
        /// <returns>executed task</returns>
        public virtual async Task<ExecutedTask> GetExecutedTaskByIdAsync(int executeTaskId)
        {
            return await _executedTaskRepository.Table.FirstOrDefaultAsync(x => x.Id == executeTaskId);
        }

        /// <summary>
        /// Get the last executed task for a report
        /// </summary>
        /// <returns>the last executed task</returns>
        public virtual async Task<ExecutedTask> GetLastExecutedTaskAsync()
        {
            return await _executedTaskRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get all executed tasks for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of executed tasks</returns>
        public virtual async Task<IPagedList<ExecutedTask>> GetAllExecutedTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<ExecutedTask>(
                await _executedTaskRepository.Table
                .OrderByDescending(x => x.Id)
                .ToListAsync(), pageIndex, pageSize);
        }

        /// <summary>
        /// Add new or update executed task entry
        /// </summary>
        /// <param name="executedTaskEntry">executed task entity</param>
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

        /// <summary>
        /// Delete executed task by identifier
        /// </summary>
        /// <param name="executedTaskId">executed task identifier</param>
        public virtual async Task DeleteExecutedTaskEntryAsync(int executedTaskId)
        {
            var executedTaskEntry = await GetExecutedTaskByIdAsync(executedTaskId);

            if (executedTaskEntry != null)
            {
                await _executedTaskRepository.DeleteAsync(executedTaskEntry);
            }
        }

        #endregion

        #region Reported messages

        /// <summary>
        /// Get reported message by identifier
        /// </summary>
        /// <param name="reportedMessageId">reported message identifier</param>
        /// <returns>reported message</returns>
        public virtual async Task<ReportedMessage> GetReportedMessageByIdAsync(int reportedMessageId)
        {
            return await _reportedMessageRepository.Table.FirstOrDefaultAsync(x => x.Id == reportedMessageId);
        }

        /// <summary>
        /// Get all reported messages by executed task identifier for a table
        /// </summary>
        /// <param name="executeTaskId">executed task identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of reported messages</returns>
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

        /// <summary>
        /// Add new or update reported message task entry
        /// </summary>
        /// <param name="reportedMessageEntry">reported message entity</param>
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

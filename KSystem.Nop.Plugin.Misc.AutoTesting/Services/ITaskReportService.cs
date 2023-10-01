namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    /// <summary>
    /// Task report service - manage executed tasks and reports
    /// </summary>
    public interface ITaskReportService
    {
        /// <summary>
        /// Get excuted task by identifier
        /// </summary>
        /// <param name="executeTaskId">executed task identifier</param>
        /// <returns>executed task</returns>
        Task<ExecutedTask> GetExecutedTaskByIdAsync(int executeTaskId);

        /// <summary>
        /// Get the last executed task for a report
        /// </summary>
        /// <returns>the last executed task</returns>
        Task<ExecutedTask> GetLastExecutedTaskAsync();

        /// <summary>
        /// Get all executed tasks for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of executed tasks</returns>
        Task<IPagedList<ExecutedTask>> GetAllExecutedTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update executed task entry
        /// </summary>
        /// <param name="executedTaskEntry">executed task entity</param>
        Task SaveExecutedTaskEntryAsync(ExecutedTask executedTaskEntry);

        /// <summary>
        /// Delete executed task by identifier
        /// </summary>
        /// <param name="executedTaskId">executed task identifier</param>
        Task DeleteExecutedTaskEntryAsync(int executeTaskId);

        /// <summary>
        /// Get reported message by identifier
        /// </summary>
        /// <param name="reportedMessageId">reported message identifier</param>
        /// <returns>reported message</returns>
        Task<ReportedMessage> GetReportedMessageByIdAsync(int reportedMessageId);

        /// <summary>
        /// Get all reported messages by executed task identifier for a table
        /// </summary>
        /// <param name="executeTaskId">executed task identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of reported messages</returns>
        Task<IPagedList<ReportedMessage>> GetAllReportedMessagesByExecutedTaskIdAsync(
            int executeTaskId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update reported message task entry
        /// </summary>
        /// <param name="reportedMessageEntry">reported message entity</param>
        Task SaveReportedMessageEntryAsync(ReportedMessage reportedMessageEntry);
    }
}

namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    public interface ITaskReportService
    {
        Task<ExecutedTask> GetExecutedTaskByIdAsync(int executeTaskId);
        Task<ExecutedTask> GetLastExecutedTaskAsync();
        Task<IPagedList<ExecutedTask>> GetAllExecutedTasksAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        Task SaveExecutedTaskEntryAsync(ExecutedTask executedTaskEntry);
        Task DeleteExecutedTaskEntryAsync(int executeTaskId);

        Task<ReportedMessage> GetReportedMessageByIdAsync(int reportedMessageId);
        Task<IPagedList<ReportedMessage>> GetAllReportedMessagesByExecutedTaskIdAsync(
            int executeTaskId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);
        Task SaveReportedMessageEntryAsync(ReportedMessage reportedMessageEntry);
    }
}

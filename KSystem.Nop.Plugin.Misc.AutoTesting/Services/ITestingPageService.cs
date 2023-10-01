namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    /// <summary>
    /// Testing page service - manage testing pages and their commands
    /// </summary>
    public interface ITestingPageService
    {
        /// <summary>
        /// Get testing page by identifier
        /// </summary>
        /// <param name="testingPageId">testing page identifier</param>
        /// <returns>testing page</returns>
        Task<TestingPage> GetTestingPageByIdAsync(int testingPageId);

        /// <summary>
        /// Get testing URL by page identifier
        /// </summary>
        /// <param name="pageId">testing page identifier</param>
        /// <returns>final testing URL</returns>
        Task<string> GetTestingUrlByPageIdAsync(int pageId);

        /// <summary>
        /// Get parameter URL delimeter by actual address
        /// </summary>
        /// <param name="testingUrl">actual testing URL address</param>
        /// <returns>parameter delimeter</returns>
        string GetTestingUrlParameterDelimeter(string testingUrl);

        /// <summary>
        /// Get all testing pages for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing pages</returns>
        Task<IPagedList<TestingPage>> GetAllTestingPagesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update testing page entry
        /// </summary>
        /// <param name="testingPageEntry">testing page entity</param>
        Task SaveTestingPageEntryAsync(TestingPage testingPageEntry);

        /// <summary>
        /// Delete testing page by identifier
        /// </summary>
        /// <param name="testingPageEntryId">testing page identifier</param>
        Task DeleteTestingPageEntryAsync(int testingPageEntryId);

        /// <summary>
        /// Get testing command by identifier
        /// </summary>
        /// <param name="testingCommandId">testing command identifier</param>
        /// <returns>testing command</returns>
        Task<TestingCommand> GetTestingCommandByIdAsync(int testingCommandId);

        /// <summary>
        /// Get all testing commands by page identifier for a table
        /// </summary>
        /// <param name="testingPageId">testing page identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing commands</returns>
        Task<IPagedList<TestingCommand>> GetAllTestingCommandsByPageIdAsync(
            int testingPageId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        /// <summary>
        /// Add new or update testing command entry
        /// </summary>
        /// <param name="testingCommand">testing command entity</param>
        Task SaveTestingCommandEntryAsync(TestingCommand testingCommand);

        /// <summary>
        /// Delete testing command by identifier
        /// </summary>
        /// <param name="testingCommandEntryId">testing command identifier</param>
        Task DeleteTestingCommandEntryAsync(int testingCommandEntryId);
    }
}

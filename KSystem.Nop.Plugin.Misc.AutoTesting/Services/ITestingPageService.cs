namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Core;

    public interface ITestingPageService
    {
        Task<TestingPage> GetTestingPageByIdAsync(int testingPageId);
        Task<string> GetTestingUrlByPageIdAsync(int pageId);
        string GetTestingUrlParameterDelimeter(string testingUrl);
        Task<IPagedList<TestingPage>> GetAllTestingPagesAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        Task SaveTestingPageEntryAsync(TestingPage testingPageEntry);
        Task DeleteTestingPageEntryAsync(int testingPageEntryId);

        Task<TestingCommand> GetTestingCommandByIdAsync(int testingCommandId);
        Task<IPagedList<TestingCommand>> GetAllTestingCommandsByPageIdAsync(
            int testingPageId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);
        Task SaveTestingCommandEntryAsync(TestingCommand testingCommand);
        Task DeleteTestingCommandEntryAsync(int testingCommandEntryId);
    }
}

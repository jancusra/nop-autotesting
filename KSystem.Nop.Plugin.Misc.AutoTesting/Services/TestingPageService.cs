namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders;

    using global::Nop.Core;
    using global::Nop.Data;

    /// <summary>
    /// Testing page service - manage testing pages and their commands
    /// </summary>
    public class TestingPageService : ITestingPageService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IWebHelper _webHelper;

        private readonly IRepository<TestingPage> _testingPageRepository;

        private readonly IRepository<TestingCommand> _testingCommandRepository;

        public TestingPageService(
            IServiceProvider serviceProvider,
            IWebHelper webHelper,
            IRepository<TestingPage> testingPageRepository,
            IRepository<TestingCommand> testingCommandRepository)
        {
            _serviceProvider = serviceProvider;
            _webHelper = webHelper;
            _testingPageRepository = testingPageRepository;
            _testingCommandRepository = testingCommandRepository;
        }

        #region Testing pages

        /// <summary>
        /// Get testing page by identifier
        /// </summary>
        /// <param name="testingPageId">testing page identifier</param>
        /// <returns>testing page</returns>
        public virtual async Task<TestingPage> GetTestingPageByIdAsync(int testingPageId)
        {
            return await _testingPageRepository.Table.FirstOrDefaultAsync(x => x.Id == testingPageId);
        }

        /// <summary>
        /// Get testing URL by page identifier
        /// </summary>
        /// <param name="pageId">testing page identifier</param>
        /// <returns>final testing URL</returns>
        public virtual async Task<string> GetTestingUrlByPageIdAsync(int pageId)
        {
            var testingPage = await GetTestingPageByIdAsync(pageId);

            if (testingPage != null)
            {
                if (!string.IsNullOrEmpty(testingPage.TestingUrl))
                {
                    var baseTestingUrl = _webHelper.GetStoreHost(_webHelper.IsCurrentConnectionSecured());

                    if (testingPage.TestingUrl.StartsWith('/'))
                    {
                        baseTestingUrl = baseTestingUrl.TrimEnd('/');
                    }

                    var testingUrl = $"{baseTestingUrl}{testingPage.TestingUrl}";
                    return testingUrl.TrimEnd('/');
                }
                else if (!string.IsNullOrEmpty(testingPage.CustomUrlProvider))
                {
                    var interfaceType = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName.StartsWith("KSystem.Nop.Plugin.Misc.AutoTesting"))
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == testingPage.CustomUrlProvider);

                    var service = _serviceProvider.GetService(interfaceType) as IBaseTestingUrlProvider;
                    return (await service.GetTestingUrlAsync(testingPage.ProviderParameters)).TrimEnd('/');
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get parameter URL delimeter by actual address
        /// </summary>
        /// <param name="testingUrl">actual testing URL address</param>
        /// <returns>parameter delimeter</returns>
        public virtual string GetTestingUrlParameterDelimeter(string testingUrl)
        {
            if (testingUrl.Contains("?"))
            {
                return "&";
            }
            else
            {
                return "?";
            }
        }

        /// <summary>
        /// Get all testing pages for a table
        /// </summary>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing pages</returns>
        public virtual async Task<IPagedList<TestingPage>> GetAllTestingPagesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<TestingPage>(
                await _testingPageRepository.Table
                .OrderBy(x => x.Name)
                .ToListAsync(), pageIndex, pageSize);
        }

        /// <summary>
        /// Add new or update testing page entry
        /// </summary>
        /// <param name="testingPageEntry">testing page entity</param>
        public virtual async Task SaveTestingPageEntryAsync(TestingPage testingPageEntry)
        {
            if (testingPageEntry.Id == default(int))
            {
                await _testingPageRepository.InsertAsync(testingPageEntry);
            }
            else
            {
                await _testingPageRepository.UpdateAsync(testingPageEntry);
            }
        }

        /// <summary>
        /// Delete testing page by identifier
        /// </summary>
        /// <param name="testingPageEntryId">testing page identifier</param>
        public virtual async Task DeleteTestingPageEntryAsync(int testingPageEntryId)
        {
            var testingPageEntry = await GetTestingPageByIdAsync(testingPageEntryId);

            if (testingPageEntry != null)
            {
                await _testingPageRepository.DeleteAsync(testingPageEntry);
            }
        }

        #endregion

        #region Testing commands

        /// <summary>
        /// Get testing command by identifier
        /// </summary>
        /// <param name="testingCommandId">testing command identifier</param>
        /// <returns>testing command</returns>
        public virtual async Task<TestingCommand> GetTestingCommandByIdAsync(int testingCommandId)
        {
            return await _testingCommandRepository.Table.FirstOrDefaultAsync(x => x.Id == testingCommandId);
        }

        /// <summary>
        /// Get all testing commands by page identifier for a table
        /// </summary>
        /// <param name="testingPageId">testing page identifier</param>
        /// <param name="pageIndex">index of the table page</param>
        /// <param name="pageSize">number of entries per one page</param>
        /// <returns>paged list of testing commands</returns>
        public virtual async Task<IPagedList<TestingCommand>> GetAllTestingCommandsByPageIdAsync(
            int testingPageId, 
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            return new PagedList<TestingCommand>(
                await _testingCommandRepository.Table
                    .Where(x => x.PageId == testingPageId)
                    .OrderBy(y => y.CommandOrder)
                    .ToListAsync(), pageIndex, pageSize);
        }

        /// <summary>
        /// Add new or update testing command entry
        /// </summary>
        /// <param name="testingCommand">testing command entity</param>
        public virtual async Task SaveTestingCommandEntryAsync(TestingCommand testingCommand)
        {
            if (testingCommand.Id == default(int))
            {
                await _testingCommandRepository.InsertAsync(testingCommand);
            }
            else
            {
                await _testingCommandRepository.UpdateAsync(testingCommand);
            }
        }

        /// <summary>
        /// Delete testing command by identifier
        /// </summary>
        /// <param name="testingCommandEntryId">testing command identifier</param>
        public virtual async Task DeleteTestingCommandEntryAsync(int testingCommandEntryId)
        {
            var testingCommandEntry = await GetTestingCommandByIdAsync(testingCommandEntryId);

            if (testingCommandEntry != null)
            {
                await _testingCommandRepository.DeleteAsync(testingCommandEntry);
            }
        }

        #endregion
    }
}

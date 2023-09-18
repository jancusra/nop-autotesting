namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders;

    using global::Nop.Core;
    using global::Nop.Data;

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

        public virtual async Task<TestingPage> GetTestingPageByIdAsync(int testingPageId)
        {
            return await _testingPageRepository.Table.FirstOrDefaultAsync(x => x.Id == testingPageId);
        }

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

        public virtual string GetTestingUrlParameterDelimeter(string testingUrl)
        {
            if(testingUrl.Contains("?"))
            {
                return "&";
            }
            else
            {
                return "?";
            }
        }

        public virtual async Task<IPagedList<TestingPage>> GetAllTestingPagesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<TestingPage>(
                await _testingPageRepository.Table
                .OrderBy(x => x.Name)
                .ToListAsync(), pageIndex, pageSize);
        }

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

        public virtual async Task<TestingCommand> GetTestingCommandByIdAsync(int testingCommandId)
        {
            return await _testingCommandRepository.Table.FirstOrDefaultAsync(x => x.Id == testingCommandId);
        }

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

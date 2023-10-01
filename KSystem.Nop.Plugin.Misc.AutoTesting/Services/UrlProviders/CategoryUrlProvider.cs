namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    /// <summary>
    /// Provides random category catalog URL
    /// </summary>
    public class CategoryUrlProvider : BaseTestingUrlProvider, ICategoryUrlProvider
    {
        private readonly ICategoryService _categoryService;

        public CategoryUrlProvider(
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get testing URL for one random category
        /// </summary>
        /// <param name="parameters">optional URL parameters</param>
        /// <returns>testing URL</returns>
        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var categoryIds = (await _categoryService.GetAllCategoriesAsync()).Select(x => x.Id).ToList();
            var randomCategorySeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(categoryIds, nameof(Category));

            if (!string.IsNullOrEmpty(randomCategorySeName))
            {
                return base.GetFinalUrlBySeName(randomCategorySeName);
            }

            return string.Empty;
        }
    }
}

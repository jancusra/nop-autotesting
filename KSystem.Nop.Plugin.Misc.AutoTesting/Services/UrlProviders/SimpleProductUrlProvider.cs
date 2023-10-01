namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    /// <summary>
    /// Provides random simple product URL
    /// </summary>
    public class SimpleProductUrlProvider : BaseTestingUrlProvider, ISimpleProductUrlProvider
    {
        private readonly IProductService _productService;

        private readonly IStoreContext _storeContext;

        public SimpleProductUrlProvider(
            IProductService productService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _productService = productService;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Get testing URL for one random simple product
        /// </summary>
        /// <param name="parameters">optional URL parameters</param>
        /// <returns>testing URL</returns>
        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var productIds = (await _productService.SearchProductsAsync(
                storeId: currentStore.Id,
                productType: ProductType.SimpleProduct,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn
            )).Select(x => x.Id).ToList();

            var randomProductSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(productIds, nameof(Product));

            if (!string.IsNullOrEmpty(randomProductSeName))
            {
                return base.GetFinalUrlBySeName(randomProductSeName);
            }

            return string.Empty;
        }
    }
}

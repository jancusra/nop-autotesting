namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    /// <summary>
    /// Provides random grouped product URL
    /// </summary>
    public class GroupedProductUrlProvider : BaseTestingUrlProvider, IGroupedProductUrlProvider
    {
        private readonly IProductService _productService;

        private readonly IStoreContext _storeContext;

        public GroupedProductUrlProvider(
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
        /// Get testing URL for one random grouped product
        /// </summary>
        /// <param name="parameters">optional URL parameters</param>
        /// <returns>testing URL</returns>
        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var productTemplateId = default(int);
            bool success = int.TryParse(base.GetQueryParameterByName(AutoTestingDefaults.ProductTemplateParameterName, parameters),
                    out productTemplateId);

            var products = (await _productService.SearchProductsAsync(
                storeId: currentStore.Id,
                productType: ProductType.GroupedProduct,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn
            )).ToList();

            if (productTemplateId > default(int))
            {
                products = products.Where(x => x.ProductTemplateId == productTemplateId).ToList();
            }

            var randomProductSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(
                products.Select(x => x.Id).ToList(), nameof(Product));

            if (!string.IsNullOrEmpty(randomProductSeName))
            {
                return base.GetFinalUrlBySeName(randomProductSeName);
            }

            return string.Empty;
        }
    }
}

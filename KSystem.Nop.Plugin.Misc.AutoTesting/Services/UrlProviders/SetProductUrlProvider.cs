namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Core.Services;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Seo;

    public class SetProductUrlProvider : BaseTestingUrlProvider, ISetProductUrlProvider
    {
        private readonly IProductCustomService _productCustomService;

        private readonly IStoreContext _storeContext;

        public SetProductUrlProvider(
            IProductCustomService productCustomService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _productCustomService = productCustomService;
            _storeContext = storeContext;
        }

        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var productIds = (await _productCustomService.SearchProductsAsync(
                storeId: currentStore.Id,
                productType: ProductType.SetProduct,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn
            )).products.Select(x => x.Id).ToList();

            var randomProductSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(productIds, nameof(Product));

            if (!string.IsNullOrEmpty(randomProductSeName))
            {
                return base.GetFinalUrlBySeName(randomProductSeName);
            }

            return string.Empty;
        }
    }
}

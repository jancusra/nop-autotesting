namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    public class OversizedProductUrlProvider : BaseTestingUrlProvider, IOversizedProductUrlProvider
    {
        private readonly IProductService _productService;

        public OversizedProductUrlProvider(
            IProductService productService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _productService = productService;
        }

        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var productIds = await _productService.GetSimpleProductIdsWithOversizedTransportAsync();
            var randomProductSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(productIds, nameof(Product));

            if (!string.IsNullOrEmpty(randomProductSeName))
            {
                return base.GetFinalUrlBySeName(randomProductSeName);
            }

            return string.Empty;
        }
    }
}

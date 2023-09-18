namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Core.Infrastructure;
    using KSystem.Nop.Core.Services;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Shared;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Data;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    public class InsuranceProductUrlProvider : BaseTestingUrlProvider, IInsuranceProductUrlProvider
    {
        private readonly ICategoryService _categoryService;

        private readonly IProductCustomService _productCustomService;

        private readonly IStoreContext _storeContext;

        private readonly string _connectionString;

        public InsuranceProductUrlProvider(
            ICategoryService categoryService,
            IProductCustomService productCustomService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base (urlRecordService,
                  webHelper,
                  workContext)
        {
            _categoryService = categoryService;
            _productCustomService = productCustomService;
            _storeContext = storeContext;
            _connectionString = DataSettingsManager.LoadSettings().ConnectionString;
        }

        private async Task<bool> InsuranceCategoryTableExistsAsync()
        {
            var sql = $"SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{AutoTestingDefaults.InsuranceCategoryTableName}'";
            var result = await SqlConnectionHelper.ExecuteCommandToListAsync<int>(_connectionString, sql);

            return result.Count > 0 && result[0] == 1;
        }

        private async Task<IList<int>> GetAllInsuranceCategoryIdsAsync()
        {
            var sql = $"SELECT [CategoryId] FROM [{AutoTestingDefaults.InsuranceCategoryTableName}]";
            return await SqlConnectionHelper.ExecuteCommandToListAsync<int>(_connectionString, sql);
        }

        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            if (await InsuranceCategoryTableExistsAsync())
            {
                var insuranceCategoryIds = await GetAllInsuranceCategoryIdsAsync();
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var categoryIds = new List<int>();
                var productTemplateId = default(int);
                bool success = int.TryParse(base.GetQueryParameterByName(AutoTestingDefaults.ProductTemplateParameterName, parameters),
                    out productTemplateId);

                foreach (var insuranceCategoryId in insuranceCategoryIds)
                {
                    if (insuranceCategoryId > default(int))
                    {
                        categoryIds.Add(insuranceCategoryId);
                        categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(insuranceCategoryId, currentStore.Id));
                    }
                }

                var productIds = (await _productCustomService.SearchProductsAsync(
                    categoryIds: categoryIds.Count > default(int) ? categoryIds : null,
                    storeId: currentStore.Id,
                    productType: ProductType.GroupedProduct,
                    productTemplateId: productTemplateId > default(int) ? productTemplateId : null,
                    visibleIndividuallyOnly: true,
                    priceMax: 100000,
                    orderBy: ProductSortingEnum.CreatedOn
                )).products.Select(x => x.Id).ToList();

                var randomProductSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(productIds, nameof(Product));

                if (!string.IsNullOrEmpty(randomProductSeName))
                {
                    return base.GetFinalUrlBySeName(randomProductSeName);
                }
            }

            return string.Empty;
        }
    }
}

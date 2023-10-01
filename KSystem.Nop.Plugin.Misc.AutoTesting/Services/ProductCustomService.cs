namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Extensions;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Data;
    using global::Nop.Services.Customers;
    using global::Nop.Services.Localization;

    /// <summary>
    /// Product custom service (by using ProductLoadAllPagedCustom SQL procedure)
    /// </summary>
    public class ProductCustomService : IProductCustomService
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly INopDataProvider _dataProvider;
        private readonly IWorkContext _workContext;

        private readonly bool _useFullTextSearch;
        private readonly int _fulltextSearchMode;

        public ProductCustomService(
            CatalogSettings catalogSettings,
            ICustomerService customerService,
            ILanguageService languageService,
            INopDataProvider dataProvider,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _languageService = languageService;
            _dataProvider = dataProvider;
            _workContext = workContext;

            _useFullTextSearch = true;
            _fulltextSearchMode = 10;
        }

        public virtual async Task<(IPagedList<Product> products, 
            IList<int> filterableSpecificationAttributeOptionIds,
            IList<int> filterableManufacturerIds,
            string filterableSpecificationAttributeOptionIdsWithCounts,
            string filterableManufacturerIdsWithCounts,
            string minMaxPriceProductIds,
            IList<int> categoriesByProductsOrKeywords)> 
            SearchProductsAsync(
            bool loadMinMaxPriceProductIds = false,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            bool loadFilterableManufacturerIds = false,
            bool loadCategoriesByProducts = false,
            bool loadCategoriesByKeywords = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            int? productTemplateId = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchManufacturerPartNumber = true,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            IList<int> filteredManufacturers = null,
            IList<int> allowedAttributeIds = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            var filterableSpecificationAttributeOptionIds = new List<int>();
            var filterableManufacturerIds = new List<int>();
            var categoriesByProductsOrKeywords = new List<int>();

            //search by keyword
            var searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync());

            //pass category identifiers as comma-delimited string
            var commaSeparatedCategoryIds = categoryIds == null ? string.Empty : string.Join(",", categoryIds);

            //pass customer role identifiers as comma-delimited string
            var commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);

            //pass allowed attribute identifiers as comma-delimited string
            var commaSeparatedAllowedAttributeIds = allowedAttributeIds == null ? string.Empty : string.Join(",", allowedAttributeIds);

            //pass specification identifiers as comma-delimited string
            var commaSeparatedSpecIds = string.Empty;
            if (filteredSpecs != null)
            {
                ((List<int>)filteredSpecs).Sort();
                commaSeparatedSpecIds = string.Join(",", filteredSpecs);
            }

            //pass manufacturer identifiers as comma-delimited string
            var commaSeparatedManufacturerIds = string.Empty;
            if (filteredManufacturers != null)
            {
                ((List<int>)filteredManufacturers).Sort();
                commaSeparatedManufacturerIds = string.Join(",", filteredManufacturers);
            }

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare input parameters
            var pCategoryIds = SqlParameterExtension.GetStringParameter("CategoryIds", commaSeparatedCategoryIds);
            var pManufacturerId = SqlParameterExtension.GetInt32Parameter("ManufacturerId", manufacturerId);
            var pStoreId = SqlParameterExtension.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
            var pVendorId = SqlParameterExtension.GetInt32Parameter("VendorId", vendorId);
            var pWarehouseId = SqlParameterExtension.GetInt32Parameter("WarehouseId", warehouseId);
            var pProductTypeId = SqlParameterExtension.GetInt32Parameter("ProductTypeId", (int?)productType);
            var pProductTemplateId = SqlParameterExtension.GetInt32Parameter("ProductTemplateId", productTemplateId);
            var pVisibleIndividuallyOnly = SqlParameterExtension.GetBooleanParameter("VisibleIndividuallyOnly", visibleIndividuallyOnly);
            var pMarkedAsNewOnly = SqlParameterExtension.GetBooleanParameter("MarkedAsNewOnly", markedAsNewOnly);
            var pProductTagId = SqlParameterExtension.GetInt32Parameter("ProductTagId", productTagId);
            var pFeaturedProducts = SqlParameterExtension.GetBooleanParameter("FeaturedProducts", featuredProducts);
            var pPriceMin = SqlParameterExtension.GetDecimalParameter("PriceMin", priceMin);
            var pPriceMax = SqlParameterExtension.GetDecimalParameter("PriceMax", priceMax);
            var pKeywords = SqlParameterExtension.GetStringParameter("Keywords", keywords);
            var pSearchDescriptions = SqlParameterExtension.GetBooleanParameter("SearchDescriptions", searchDescriptions);
            var pSearchManufacturerPartNumber = SqlParameterExtension.GetBooleanParameter("SearchManufacturerPartNumber", searchManufacturerPartNumber);
            var pSearchSku = SqlParameterExtension.GetBooleanParameter("SearchSku", searchSku);
            var pSearchProductTags = SqlParameterExtension.GetBooleanParameter("SearchProductTags", searchProductTags);
            var pUseFullTextSearch = SqlParameterExtension.GetBooleanParameter("UseFullTextSearch", _useFullTextSearch);
            var pFullTextMode = SqlParameterExtension.GetInt32Parameter("FullTextMode", _fulltextSearchMode);
            var pFilteredSpecs = SqlParameterExtension.GetStringParameter("FilteredSpecs", commaSeparatedSpecIds);
            var pFilteredManufacurers = SqlParameterExtension.GetStringParameter("FilteredManufacturers", commaSeparatedManufacturerIds);
            var pLanguageId = SqlParameterExtension.GetInt32Parameter("LanguageId", searchLocalizedValue ? languageId : 0);
            var pOrderBy = SqlParameterExtension.GetInt32Parameter("OrderBy", (int)orderBy);
            var pAllowedCustomerRoleIds = SqlParameterExtension.GetStringParameter("AllowedCustomerRoleIds", !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : string.Empty);
            var pAllowedAttributeIds = SqlParameterExtension.GetStringParameter("AllowedAttributeIds", commaSeparatedAllowedAttributeIds);
            var pPageIndex = SqlParameterExtension.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterExtension.GetInt32Parameter("PageSize", pageSize);
            var pShowHidden = SqlParameterExtension.GetBooleanParameter("ShowHidden", showHidden);
            var pOverridePublished = SqlParameterExtension.GetBooleanParameter("OverridePublished", overridePublished);
            var pLoadMinMaxPriceProductIds = SqlParameterExtension.GetBooleanParameter("LoadMinMaxPriceProductIds", loadMinMaxPriceProductIds);
            var pLoadFilterableSpecificationAttributeOptionIds = SqlParameterExtension.GetBooleanParameter("LoadFilterableSpecificationAttributeOptionIds", loadFilterableSpecificationAttributeOptionIds);
            var pLoadFilterableManufacturerIds = SqlParameterExtension.GetBooleanParameter("LoadFilterableManufacturerIds", loadFilterableManufacturerIds);
            var pLoadCategoriesByProducts = SqlParameterExtension.GetBooleanParameter("LoadCategoriesByProducts", loadCategoriesByProducts);
            var pLoadCategoriesByKeywords = SqlParameterExtension.GetBooleanParameter("LoadCategoriesByKeywords", loadCategoriesByKeywords);

            //prepare output parameters
            var pMinMaxPriceProductIds = SqlParameterExtension.GetOutputStringParameter("MinMaxPriceProductIds");
            pMinMaxPriceProductIds.Size = int.MaxValue - 1;

            var pFilterableSpecificationAttributeOptionIds = SqlParameterExtension.GetOutputStringParameter("FilterableSpecificationAttributeOptionIds");
            pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
            var pFilterableManufacturerIds = SqlParameterExtension.GetOutputStringParameter("FilterableManufacturerIds");
            pFilterableManufacturerIds.Size = int.MaxValue - 1;

            var pFilterableSpecificationAttributeOptionIdsWithCounts = SqlParameterExtension.GetOutputStringParameter("FilterableSpecificationAttributeOptionIdsWithCounts");
            pFilterableSpecificationAttributeOptionIdsWithCounts.Size = int.MaxValue - 1;
            var pFilterableManufacturerIdsWithCounts = SqlParameterExtension.GetOutputStringParameter("FilterableManufacturerIdsWithCounts");
            pFilterableManufacturerIdsWithCounts.Size = int.MaxValue - 1;

            var pCategoriesByProductsOrKeywords = SqlParameterExtension.GetOutputStringParameter("CategoriesByProductsOrKeywords");
            pCategoriesByProductsOrKeywords.Size = int.MaxValue - 1;
            var pTotalRecords = SqlParameterExtension.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var products = await (await _dataProvider.QueryProcAsync<Product>("ProductLoadAllPagedCustom",
                pCategoryIds,
                pManufacturerId,
                pStoreId,
                pVendorId,
                pWarehouseId,
                pMarkedAsNewOnly,
                pOverridePublished,
                pProductTypeId,
                pProductTemplateId,
                pVisibleIndividuallyOnly,
                pProductTagId,
                pFeaturedProducts,
                pPriceMin,
                pPriceMax,
                pKeywords,
                pSearchDescriptions,
                pSearchManufacturerPartNumber,
                pSearchSku,
                pSearchProductTags,
                pUseFullTextSearch,
                pFullTextMode,
                pFilteredSpecs,
                pFilteredManufacurers,
                pLanguageId,
                pOrderBy,
                pAllowedCustomerRoleIds,
                pAllowedAttributeIds,
                pPageIndex,
                pPageSize,
                pShowHidden,
                pLoadMinMaxPriceProductIds,
                pLoadFilterableSpecificationAttributeOptionIds,
                pLoadFilterableManufacturerIds,
                pLoadCategoriesByProducts,
                pLoadCategoriesByKeywords,
                pMinMaxPriceProductIds,
                pFilterableSpecificationAttributeOptionIds,
                pFilterableManufacturerIds,
                pFilterableSpecificationAttributeOptionIdsWithCounts,
                pFilterableManufacturerIdsWithCounts,
                pCategoriesByProductsOrKeywords,
                pTotalRecords)).ToListAsync();

            //get filterable specification attribute option identifier
            var filterableSpecificationAttributeOptionIdsStr =
                pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value
                    ? (string)pFilterableSpecificationAttributeOptionIds.Value
                    : string.Empty;

            if (loadFilterableSpecificationAttributeOptionIds &&
                !string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
            {
                filterableSpecificationAttributeOptionIds = filterableSpecificationAttributeOptionIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
            }

            var filterableSpecificationAttributeOptionIdsWithCounts =
                pFilterableSpecificationAttributeOptionIdsWithCounts.Value != DBNull.Value
                    ? (string)pFilterableSpecificationAttributeOptionIdsWithCounts.Value
                    : string.Empty;

            //get filterable manufacturer identifier
            var filterableManufacturerIdsStr =
                pFilterableManufacturerIds.Value != DBNull.Value
                    ? (string)pFilterableManufacturerIds.Value
                    : string.Empty;

            if (loadFilterableManufacturerIds &&
                !string.IsNullOrWhiteSpace(filterableManufacturerIdsStr))
            {
                filterableManufacturerIds = filterableManufacturerIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
            }

            var filterableManufacturerIdsWithCounts =
                pFilterableManufacturerIdsWithCounts.Value != DBNull.Value
                    ? (string)pFilterableManufacturerIdsWithCounts.Value
                    : string.Empty;

            //get min-max price product ids
            var minMaxPriceProductIds =
                pMinMaxPriceProductIds.Value != DBNull.Value
                    ? (string)pMinMaxPriceProductIds.Value
                    : string.Empty;

            //get categories by products or keywords identifiers
            var categoriesByProductsOrKeywordsIdsStr =
                pCategoriesByProductsOrKeywords.Value != DBNull.Value
                    ? (string)pCategoriesByProductsOrKeywords.Value
                    : string.Empty;

            if ((loadCategoriesByProducts || loadCategoriesByKeywords) &&
                !string.IsNullOrWhiteSpace(categoriesByProductsOrKeywordsIdsStr))
            {
                categoriesByProductsOrKeywords = categoriesByProductsOrKeywordsIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
            }

            //return products
            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return (new PagedList<Product>(products, pageIndex, pageSize, totalRecords),
                filterableSpecificationAttributeOptionIds,
                filterableManufacturerIds,
                filterableSpecificationAttributeOptionIdsWithCounts,
                filterableManufacturerIdsWithCounts,
                minMaxPriceProductIds,
                categoriesByProductsOrKeywords);
        }
    }
}

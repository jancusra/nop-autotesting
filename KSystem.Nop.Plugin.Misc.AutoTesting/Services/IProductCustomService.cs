namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;

    /// <summary>
    /// Product custom service (by using ProductLoadAllPagedCustom SQL procedure)
    /// </summary>
    public interface IProductCustomService
    {
        Task<(IPagedList<Product> products,
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
            bool? overridePublished = null);
    }
}

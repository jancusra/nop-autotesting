/****** Object:  StoredProcedure [dbo].[ProductLoadAllPagedCustom] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('[dbo].[ProductLoadAllPagedCustom]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[ProductLoadAllPagedCustom] 
END 
GO

-- update the "ProductLoadAllPagedCustom" stored procedure
CREATE PROCEDURE [dbo].[ProductLoadAllPagedCustom]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@ProductTemplateId	int = null, --custom parameter - product template identifier, null - load all
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@FilteredManufacturers	nvarchar(MAX) = null, --filter by manufacturers (comma-separated list of IDs). e.g. 14,15,16 (custom parameter)
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
	@AllowedAttributeIds	nvarchar(MAX) = null,	--custom parameter - only allowed attributes to specification outputs
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" product
	@LoadMinMaxPriceProductIds bit = 0, -- load products with min max prices in category (without filters)
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@LoadFilterableManufacturerIds bit = 0, -- custom manufacturer parameter
	@LoadCategoriesByProducts bit = 0, -- custom parameter to load categories (where output products are included)
	@LoadCategoriesByKeywords bit = 0, -- custom parameter to load categories by inserted search term
	@MinMaxPriceProductIds nvarchar(MAX) = null OUTPUT, -- products with min max prices in category (without filters)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@FilterableManufacturerIds nvarchar(MAX) = null OUTPUT, -- custom manufacturer parameter
	@FilterableSpecificationAttributeOptionIdsWithCounts nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers (with product counts)
	@FilterableManufacturerIdsWithCounts nvarchar(MAX) = null OUTPUT, -- custom manufacturer parameter (with product counts)
	@CategoriesByProductsOrKeywords nvarchar(MAX) = null OUTPUT, -- custom output category IDs by inserted configuration
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max),
		@sql_category_map nvarchar(max),
		@sql_manufacturer_map nvarchar(max),
		@sql_temp nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
                    DECLARE @len_keywords INT
					DECLARE @len_nvarchar INT
					SET @len_keywords = 0
					SET @len_nvarchar = DATALENGTH(CONVERT(NVARCHAR(MAX), 'a'))

					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

                        --LEN excludes trailing spaces. That is why we use DATALENGTH
						--see https://docs.microsoft.com/sql/t-sql/functions/len-transact-sql?view=sqlallproducts-allversions for more ditails
						SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, @len_keywords - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
                    SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, @len_keywords) + '*"'
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + '(CONTAINS(p.[Name], @Keywords) AND p.[VisibleIndividually] = 1) '
		ELSE
			SET @sql = @sql + '(PATINDEX(@Keywords, p.[Name]) > 0 AND p.[VisibleIndividually] = 1) '

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			IF @UseFullTextSearch = 1
			BEGIN
				SET @sql = @sql + 'OR (CONTAINS(p.[ShortDescription], @Keywords) AND p.[VisibleIndividually] = 1) '
				SET @sql = @sql + 'OR (CONTAINS(p.[FullDescription], @Keywords) AND p.[VisibleIndividually] = 1) '
			END
			ELSE
			BEGIN
				SET @sql = @sql + 'OR (PATINDEX(@Keywords, p.[ShortDescription]) > 0 AND p.[VisibleIndividually] = 1) '
				SET @sql = @sql + 'OR (PATINDEX(@Keywords, p.[FullDescription]) > 0 AND p.[VisibleIndividually] = 1) '
			END
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + 'OR p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + 'OR p.[Sku] = @OriginalKeywords '
		END

		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND ( (lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

		IF @SearchDescriptions = 1
		BEGIN
			--localized product short description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			--localized product full description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
		END

		SET @sql = @sql + ' ) '

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by specification options
	CREATE TABLE #FilteredSpecificationAttributeOptions
	(
		SpecificationAttributeOptionId int not null unique
	)
	INSERT INTO #FilteredSpecificationAttributeOptions (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecificationAttributesCount int
	SET @SpecificationAttributesCount = (SELECT COUNT(DISTINCT sao.SpecificationAttributeId) FROM #FilteredSpecificationAttributeOptions fs
		INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId)

	CREATE TABLE #FilteredSpecificationAttributes
	(
		AttributeId int not null
	)
	CREATE UNIQUE CLUSTERED INDEX IX_#FilteredSpecificationAttributes_AttributeId ON #FilteredSpecificationAttributes (AttributeId);
	INSERT INTO #FilteredSpecificationAttributes SELECT DISTINCT sap.SpecificationAttributeId FROM SpecificationAttributeOption sap
		INNER JOIN #FilteredSpecificationAttributeOptions fs ON fs.SpecificationAttributeOptionId = sap.Id

	--filter by manufacturer IDs (custom)
	SET @FilteredManufacturers = isnull(@FilteredManufacturers, '')	
	CREATE TABLE #FilteredManufacturers
	(
		ManufacturerId int not null
	)
	INSERT INTO #FilteredManufacturers (ManufacturerId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredManufacturers, ',')	
	DECLARE @ManufacturersCount int	
	SET @ManufacturersCount = (SELECT COUNT(1) FROM #FilteredManufacturers)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount int
	SET @FilteredCustomerRoleIdsCount = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)

	--allowed attribute ids
	SET @AllowedAttributeIds = isnull(@AllowedAttributeIds, '')
	CREATE TABLE #AllowedAttributeIds
	(
		AttributeId int not null
	)
	INSERT INTO #AllowedAttributeIds (AttributeId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedAttributeIds, ',')
	DECLARE @AllowedAttributeIdsCount int
	SET @AllowedAttributeIdsCount = (SELECT COUNT(1) FROM #AllowedAttributeIds)

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #ProductsFilteredByCategories 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
	INSERT INTO #ProductsFilteredByCategories ([ProductId])
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'

	SET @sql_category_map = ''
	SET @sql_manufacturer_map = ''
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql_category_map = '
			INNER JOIN Product_Category_Mapping pcm with (NOLOCK) ON p.Id = pcm.ProductId'
		SET @sql = @sql + @sql_category_map
	END
	
	IF (@ManufacturerId > 0 OR @ManufacturersCount > 0)
	BEGIN
		SET @sql_manufacturer_map = '
			INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.Id = pmm.ProductId'
		SET @sql = @sql + @sql_manufacturer_map
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END

	--filter by product template
	IF @ProductTemplateId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTemplateId = ' + CAST(@ProductTemplateId AS nvarchar(max))
	END
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FilteredCustomerRoleIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END

	EXEC sp_executesql @sql

	IF @LoadMinMaxPriceProductIds = 1
	BEGIN
		CREATE TABLE #ProductPricesTemp
		(
			ProductId int not null,
			Price decimal(18,4) not null,
			TierPrice decimal(18,4) null
		)

		INSERT INTO #ProductPricesTemp (ProductId, Price, TierPrice)
		SELECT pfbc.ProductId, p.Price, tpr.Price FROM #ProductsFilteredByCategories pfbc
			INNER JOIN Product p ON p.Id = pfbc.ProductId
			LEFT JOIN TierPrice tpr with (NOLOCK) ON tpr.ProductId = pfbc.ProductId 
			AND tpr.Quantity = 1
			AND tpr.CustomerRoleId IN (SELECT CustomerRoleId FROM #FilteredCustomerRoleIds)
			AND (tpr.StoreId = 0 OR tpr.StoreId = @StoreId)
			AND (tpr.StartDateTimeUtc IS NULL OR tpr.StartDateTimeUtc <= SYSUTCDATETIME())
			AND (tpr.EndDateTimeUtc IS NULL OR tpr.EndDateTimeUtc >= SYSUTCDATETIME())
			
		DECLARE @MinPrice decimal(18,4) = NULL
		DECLARE @MaxPrice decimal(18,4) = NULL
		DECLARE @MinTierPrice decimal(18,4) = NULL
		DECLARE @MaxTierPrice decimal(18,4) = NULL
		DECLARE @MinProductId int = 0
		DECLARE @MaxProductId int = 0

		SET @MinPrice = (SELECT MIN(Price) FROM #ProductPricesTemp)
		SET @MaxPrice = (SELECT MAX(Price) FROM #ProductPricesTemp)
		SET @MinTierPrice = (SELECT MIN(TierPrice) FROM #ProductPricesTemp WHERE TierPrice IS NOT NULL)
		SET @MaxTierPrice = (SELECT MAX(TierPrice) FROM #ProductPricesTemp WHERE TierPrice IS NOT NULL)

		IF (@MinPrice IS NOT NULL AND @MinTierPrice IS NOT NULL AND @MinTierPrice < @MinPrice)
		BEGIN
			SET @MinProductId = (SELECT TOP 1 ProductId FROM #ProductPricesTemp WHERE TierPrice = @MinTierPrice)
		END
		ELSE IF (@MinPrice IS NOT NULL)
		BEGIN
			SET @MinProductId = (SELECT TOP 1 ProductId FROM #ProductPricesTemp WHERE Price = @MinPrice)
		END

		IF (@MaxPrice IS NOT NULL AND @MaxTierPrice IS NOT NULL AND @MaxTierPrice > @MaxPrice)
		BEGIN
			SET @MaxProductId = (SELECT TOP 1 ProductId FROM #ProductPricesTemp WHERE TierPrice = @MaxTierPrice)
		END
		ELSE IF (@MaxPrice IS NOT NULL)
		BEGIN
			SET @MaxProductId = (SELECT TOP 1 ProductId FROM #ProductPricesTemp WHERE Price = @MaxPrice)
		END

		SELECT @MinMaxPriceProductIds = CAST(@MinProductId as nvarchar(4000)) + ',' + CAST(@MaxProductId as nvarchar(4000))

		DROP TABLE #ProductPricesTemp
	END

	CREATE TABLE #ProductsFilteredByPrices 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
		INSERT INTO #ProductsFilteredByPrices ([ProductId])
		SELECT pfbc.ProductId FROM #ProductsFilteredByCategories pfbc with (NOLOCK)
		INNER JOIN Product p with (NOLOCK) ON p.Id = pfbc.ProductId'
	SET @sql_temp = 'WHERE'

	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		WHERE (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
		SET @sql_temp = 'AND'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		' + @sql_temp + ' (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END

	EXEC sp_executesql @sql

	--filter by specification options
	CREATE TABLE #ProductIdsBeforeFiltersApplied
	(
		[ProductId] int NOT NULL
	)
	CREATE UNIQUE CLUSTERED INDEX IX_ProductIds_ProductId ON #ProductIdsBeforeFiltersApplied (ProductId)
	INSERT INTO #ProductIdsBeforeFiltersApplied ([ProductId])
	SELECT ProductId FROM #ProductsFilteredByPrices GROUP BY ProductId ORDER BY min([Id])

	CREATE TABLE #FilteredSpecificationAttributesToProduct
	(
		ProductId int not null,
		AttributesCount int not null
	)
	CREATE UNIQUE CLUSTERED INDEX IX_#FilteredSpecificationAttributesToProduct_ProductId ON #FilteredSpecificationAttributesToProduct (ProductId)

	IF @SpecificationAttributesCount > 0
	BEGIN
		IF @SpecificationAttributesCount > 1
		BEGIN
			INSERT INTO #FilteredSpecificationAttributesToProduct
			SELECT psm.ProductId, COUNT (DISTINCT sao.SpecificationAttributeId) FROM Product_SpecificationAttribute_Mapping psm
				INNER JOIN #ProductIdsBeforeFiltersApplied p ON p.ProductId = psm.ProductId
				INNER JOIN #FilteredSpecificationAttributeOptions fs ON fs.SpecificationAttributeOptionId = psm.SpecificationAttributeOptionId
				INNER JOIN SpecificationAttributeOption sao ON sao.Id = psm.SpecificationAttributeOptionId
			GROUP BY psm.ProductId HAVING COUNT (DISTINCT sao.SpecificationAttributeId) >= @SpecificationAttributesCount - 1
		END
		IF @SpecificationAttributesCount = 1
		BEGIN
			INSERT INTO #FilteredSpecificationAttributesToProduct
			SELECT DISTINCT psm.ProductId, 1 FROM Product_SpecificationAttribute_Mapping psm
				INNER JOIN #ProductIdsBeforeFiltersApplied p ON p.ProductId = psm.ProductId
				INNER JOIN #FilteredSpecificationAttributeOptions fs ON fs.SpecificationAttributeOptionId = psm.SpecificationAttributeOptionId AND psm.AllowFiltering = 1
			INSERT INTO #FilteredSpecificationAttributesToProduct
			SELECT DISTINCT psm.ProductId, 0 FROM Product_SpecificationAttribute_Mapping psm
				INNER JOIN #ProductIdsBeforeFiltersApplied p ON p.ProductId = psm.ProductId
				INNER JOIN SpecificationAttributeOption sao ON sao.Id = psm.SpecificationAttributeOptionId
				INNER JOIN #FilteredSpecificationAttributes fsa ON fsa.AttributeId = sao.SpecificationAttributeId
			WHERE NOT EXISTS (SELECT NULL FROM #FilteredSpecificationAttributesToProduct fsatp WHERE fsatp.ProductId = psm.ProductId) AND psm.AllowFiltering = 1
		END
		IF @SpecificationAttributesCount > 1
		BEGIN
			DELETE #FilteredSpecificationAttributesToProduct FROM #FilteredSpecificationAttributesToProduct fsatp
			WHERE (SELECT COUNT (DISTINCT sao.SpecificationAttributeId) FROM Product_SpecificationAttribute_Mapping psm
				INNER JOIN SpecificationAttributeOption sao ON sao.Id = psm.SpecificationAttributeOptionId
				INNER JOIN #FilteredSpecificationAttributes fsa ON fsa.AttributeId = sao.SpecificationAttributeId
			WHERE psm.ProductId = fsatp.ProductId) < @SpecificationAttributesCount
		END
	END

	CREATE TABLE #ProductsFilteredFinal 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
		INSERT INTO #ProductsFilteredFinal ([ProductId])
		SELECT pfbp.ProductId FROM #ProductsFilteredByPrices pfbp with (NOLOCK)
		INNER JOIN Product p with (NOLOCK) ON p.Id = pfbp.ProductId'

	SET @sql = @sql + @sql_category_map
	SET @sql = @sql + @sql_manufacturer_map
	SET @sql_temp = 'WHERE'

	IF @SpecificationAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		WHERE ((SELECT AttributesCount FROM #FilteredSpecificationAttributesToProduct fsatp
			WHERE p.Id = fsatp.ProductId) = ' + CAST(@SpecificationAttributesCount AS nvarchar(max)) + ')'
		SET @sql_temp = 'AND'
	END

	IF @ManufacturersCount > 0
	BEGIN
		SET @sql = @sql + '
			' + @sql_temp + ' pmm.ManufacturerId IN (SELECT ManufacturerId FROM #FilteredManufacturers)'
	END

	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
		ORDER BY' + @sql_orderby

	EXEC sp_executesql @sql

	CREATE TABLE #PageIndex (
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId FROM #ProductsFilteredFinal GROUP BY ProductId ORDER BY min([Id])
	SET @TotalRecords = @@rowcount

	--parse filterable specification option ids
	IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN
		CREATE TABLE #PotentialProductSpecificationAttributeIds
		(
			[ProductId] int NOT NULL,
			[AttributeId] int NOT NULL,
			[SpecificationAttributeOptionId] int NOT NULL
		)
		INSERT INTO #PotentialProductSpecificationAttributeIds ([ProductId], [AttributeId], [SpecificationAttributeOptionId])
		SELECT psm.ProductId, sao.SpecificationAttributeId, psm.SpecificationAttributeOptionId FROM Product_SpecificationAttribute_Mapping psm
			INNER JOIN #FilteredSpecificationAttributesToProduct fsatp on fsatp.ProductId = psm.ProductId
			INNER JOIN SpecificationAttributeOption sao ON sao.Id = psm.SpecificationAttributeOptionId
			INNER JOIN #FilteredSpecificationAttributes fsa ON fsa.AttributeId = sao.SpecificationAttributeId
		WHERE fsatp.AttributesCount = @SpecificationAttributesCount - 1 AND sao.SpecificationAttributeId NOT IN
		(SELECT sao.SpecificationAttributeId FROM Product_SpecificationAttribute_Mapping psm1
			INNER JOIN SpecificationAttributeOption sao1 ON sao1.Id = psm1.SpecificationAttributeOptionId
			INNER JOIN #FilteredSpecificationAttributeOptions fs ON fs.SpecificationAttributeOptionId = sao.Id
		WHERE psm1.ProductId = psm.ProductId)

		IF @ManufacturersCount > 0
		BEGIN
			DELETE FROM #PotentialProductSpecificationAttributeIds
			WHERE NOT EXISTS (SELECT NULL FROM Product_Manufacturer_Mapping [pmm]
				INNER JOIN #FilteredManufacturers [fm] ON [fm].ManufacturerId = [pmm].ManufacturerId
			WHERE [pmm].ProductId = #PotentialProductSpecificationAttributeIds.ProductId)
		END

		CREATE TABLE #FilterableSpecs
		(
			[ProductId] int NOT NULL,
			[AttributeId] int NOT NULL,
			[SpecificationAttributeOptionId] int NOT NULL
		)
		CREATE TABLE #FilterableSpecsDistinct
		(
			[AttributeId] int NOT NULL,
			[SpecificationAttributeOptionId] int NOT NULL
		)
		CREATE TABLE #FilterableSpecsWithCounts
		(
			[SpecificationAttributeOptionId] int NOT NULL,
			[ProductsCount] int NOT NULL
		)

		IF @AllowedAttributeIdsCount > 0
		BEGIN
			INSERT INTO #FilterableSpecs ([ProductId], [AttributeId], [SpecificationAttributeOptionId])
			SELECT DISTINCT [psam].ProductId, [sao].SpecificationAttributeId, [psam].SpecificationAttributeOptionId
			FROM [Product_SpecificationAttribute_Mapping] [psam] with (NOLOCK)
				INNER JOIN SpecificationAttributeOption [sao] ON sao.Id = psam.SpecificationAttributeOptionId
			WHERE [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi]) AND [psam].[AllowFiltering] = 1
				AND [sao].SpecificationAttributeId IN (SELECT AttributeId FROM #AllowedAttributeIds)

			INSERT INTO #FilterableSpecs ([ProductId], [AttributeId], [SpecificationAttributeOptionId])
			SELECT DISTINCT ProductId, AttributeId, SpecificationAttributeOptionId FROM #PotentialProductSpecificationAttributeIds
			WHERE AttributeId IN (SELECT AttributeId FROM #AllowedAttributeIds)
		END
		ELSE
		BEGIN
			INSERT INTO #FilterableSpecs ([ProductId], [AttributeId], [SpecificationAttributeOptionId])
			SELECT DISTINCT [psam].ProductId, [sao].SpecificationAttributeId, [psam].SpecificationAttributeOptionId
			FROM [Product_SpecificationAttribute_Mapping] [psam] with (NOLOCK)
				INNER JOIN SpecificationAttributeOption [sao] ON sao.Id = psam.SpecificationAttributeOptionId
			WHERE [psam].[ProductId] IN (SELECT [pi].ProductId FROM #PageIndex [pi]) AND [psam].[AllowFiltering] = 1

			INSERT INTO #FilterableSpecs ([ProductId], [AttributeId], [SpecificationAttributeOptionId])
			SELECT DISTINCT ProductId, AttributeId, SpecificationAttributeOptionId FROM #PotentialProductSpecificationAttributeIds
		END

		INSERT INTO #FilterableSpecsDistinct ([AttributeId], [SpecificationAttributeOptionId])
		SELECT DISTINCT AttributeId, SpecificationAttributeOptionId FROM #FilterableSpecs

		IF @SpecificationAttributesCount > 1
		BEGIN
			WHILE EXISTS (SELECT * FROM #FilterableSpecsDistinct)
			BEGIN
				DECLARE @AttributeOptionId int
				SET @AttributeOptionId = (SELECT TOP 1 SpecificationAttributeOptionId FROM #FilterableSpecsDistinct)

				IF EXISTS (SELECT * FROM #FilteredSpecificationAttributeOptions WHERE SpecificationAttributeOptionId = @AttributeOptionId)
				BEGIN
					INSERT INTO #FilterableSpecsWithCounts ([SpecificationAttributeOptionId], [ProductsCount])
					VALUES (@AttributeOptionId, (SELECT count(*) FROM #FilterableSpecs WHERE SpecificationAttributeOptionId = @AttributeOptionId))
				END
				ELSE
				BEGIN
					DECLARE @AttributeId int
					DECLARE @FilteredOptionsCount int
					DECLARE @ProductsCount int

					CREATE TABLE #FilteredOptions
					(
						[AttributeId] int NOT NULL,
						[OptionId] int NOT NULL
					)

					SET @AttributeId = (SELECT TOP 1 AttributeId FROM #FilterableSpecsDistinct)

					INSERT INTO #FilteredOptions (AttributeId, OptionId) 
					SELECT sao.SpecificationAttributeId, fsao.SpecificationAttributeOptionId FROM #FilteredSpecificationAttributeOptions fsao
						INNER JOIN SpecificationAttributeOption sao ON sao.Id = fsao.SpecificationAttributeOptionId
					WHERE sao.SpecificationAttributeId != @AttributeId

					SET @FilteredOptionsCount = (SELECT count(*) FROM #FilteredOptions)

					IF @FilteredOptionsCount > 0
					BEGIN
						DECLARE @NotValidProductIds TABLE(ProductId int)
						DECLARE @FilteredAttributeIds TABLE(AttributeId int)

						INSERT INTO @FilteredAttributeIds (AttributeId)
							SELECT DISTINCT AttributeId FROM #FilteredOptions

						WHILE EXISTS (SELECT * FROM @FilteredAttributeIds)
						BEGIN
							DECLARE @FilteringAttributeId int
							SET @FilteringAttributeId = (SELECT TOP 1 AttributeId FROM @FilteredAttributeIds)

							INSERT INTO @NotValidProductIds (ProductId)
								SELECT ProductId FROM #PotentialProductSpecificationAttributeIds ppsa
								GROUP BY ppsa.ProductId HAVING (SELECT count(*) FROM Product_SpecificationAttribute_Mapping fs 
									WHERE fs.ProductId = ppsa.ProductId AND fs.SpecificationAttributeOptionId IN 
									(SELECT OptionId FROM #FilteredOptions WHERE AttributeId = @FilteringAttributeId)) = 0

							DELETE FROM @FilteredAttributeIds WHERE AttributeId = @FilteringAttributeId
						END

						SET @ProductsCount = (SELECT count(*) FROM #FilterableSpecs 
							WHERE SpecificationAttributeOptionId = @AttributeOptionId AND ProductId NOT IN(SELECT ProductId FROM @NotValidProductIds))

						IF @ProductsCount > 0
						BEGIN
							INSERT INTO #FilterableSpecsWithCounts ([SpecificationAttributeOptionId], [ProductsCount])
								VALUES (@AttributeOptionId, @ProductsCount)
						END

						DELETE FROM @NotValidProductIds
					END
					ELSE
					BEGIN
						SET @ProductsCount = (SELECT count(*) FROM #FilterableSpecs WHERE SpecificationAttributeOptionId = @AttributeOptionId)

						IF @ProductsCount > 0
						BEGIN
							INSERT INTO #FilterableSpecsWithCounts ([SpecificationAttributeOptionId], [ProductsCount])
							VALUES (@AttributeOptionId, @ProductsCount)
						END
					END

					DROP TABLE #FilteredOptions
				END
			 
				DELETE FROM #FilterableSpecsDistinct WHERE SpecificationAttributeOptionId = @AttributeOptionId
			END
		END
		ELSE
		BEGIN
			INSERT INTO #FilterableSpecsWithCounts ([SpecificationAttributeOptionId], [ProductsCount])
				SELECT SpecificationAttributeOptionId, COUNT(SpecificationAttributeOptionId) FROM #FilterableSpecs
				GROUP BY SpecificationAttributeOptionId
		END

		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(MAX))
			FROM #FilterableSpecsWithCounts

		SELECT @FilterableSpecificationAttributeOptionIdsWithCounts = COALESCE(@FilterableSpecificationAttributeOptionIdsWithCounts + ',' , '') 
			+ CAST(SpecificationAttributeOptionId as nvarchar(MAX)) + '-' + CAST(ProductsCount as nvarchar(MAX)) FROM #FilterableSpecsWithCounts

		DROP TABLE #FilterableSpecs
		DROP TABLE #FilterableSpecsDistinct
		DROP TABLE #FilterableSpecsWithCounts
		DROP TABLE #PotentialProductSpecificationAttributeIds
	END

	--parse filterable manufacturer ids
	IF @LoadFilterableManufacturerIds = 1
	BEGIN
		CREATE TABLE #FilterableManufacturers
		(
			[ProductId] int NOT NULL,
			[ManufacturerId] int NOT NULL
		)
		CREATE TABLE #FilterableManufacturersWithCounts
		(
			[ManufacturerId] int NOT NULL,
			[ProductsCount] int NOT NULL
		)

		INSERT INTO #FilterableManufacturers ([ProductId], [ManufacturerId])
		SELECT DISTINCT [pmm].ProductId, [pmm].ManufacturerId FROM Product_Manufacturer_Mapping [pmm]
			INNER JOIN #ProductIdsBeforeFiltersApplied ON #ProductIdsBeforeFiltersApplied.ProductId = [pmm].ProductId

		IF @SpecificationAttributesCount > 0
		BEGIN
			DELETE FROM #FilterableManufacturers FROM #FilterableManufacturers fm
				LEFT JOIN #FilteredSpecificationAttributesToProduct fsatp ON fsatp.ProductId = fm.ProductId
			WHERE fsatp.ProductId IS NULL OR fsatp.AttributesCount != @SpecificationAttributesCount
		END

		INSERT INTO #FilterableManufacturersWithCounts ([ManufacturerId], [ProductsCount])
		SELECT ManufacturerId, COUNT(ManufacturerId) FROM #FilterableManufacturers
		GROUP BY ManufacturerId

		SELECT @FilterableManufacturerIds = COALESCE(@FilterableManufacturerIds + ',' , '') + CAST(ManufacturerId as nvarchar(MAX)) FROM #FilterableManufacturersWithCounts

		SELECT @FilterableManufacturerIdsWithCounts = COALESCE(@FilterableManufacturerIdsWithCounts + ',' , '') 
			+ CAST(ManufacturerId as nvarchar(MAX)) + '-' + CAST(ProductsCount as nvarchar(MAX)) FROM #FilterableManufacturersWithCounts

		DROP TABLE #FilterableManufacturers
		DROP TABLE #FilterableManufacturersWithCounts
	END

	--get categories by products or keywords
	IF @LoadCategoriesByProducts = 1 OR @LoadCategoriesByKeywords = 1
	BEGIN
		CREATE TABLE #OutputCategoryIds
		(
			[CategoryId] int not null
		)
		CREATE TABLE #OutputCategoryIdsDistinct
		(
			[CategoryId] int NOT NULL
		)

		IF @LoadCategoriesByProducts = 1
		BEGIN
			INSERT INTO #OutputCategoryIds ([CategoryId])
			SELECT PCM.CategoryId FROM Product_Category_Mapping PCM
			WHERE PCM.ProductId IN (SELECT ProductId FROM #PageIndex)
		END

		IF @LoadCategoriesByKeywords = 1
		BEGIN
			SET @sql = '
			INSERT INTO #OutputCategoryIds ([CategoryId])
			SELECT c.Id
			FROM Category c with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(c.[Name], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, c.[Name]) > 0 '

			IF @SearchDescriptions = 1
			BEGIN
				IF @UseFullTextSearch = 1
				BEGIN
					SET @sql = @sql + 'OR CONTAINS(c.[Description], @Keywords) '
				END
				ELSE
				BEGIN
					SET @sql = @sql + 'OR PATINDEX(@Keywords, c.[Description]) > 0 '
				END
			END

			--localized category name
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Category''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND ( (lp.LocaleKey = N''Name'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			IF @SearchDescriptions = 1
			BEGIN
				SET @sql = @sql + '
					OR (lp.LocaleKey = N''Description'''
				IF @UseFullTextSearch = 1
					SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
				ELSE
					SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
			END

			SET @sql = @sql + ' ) '

			EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords
		END

		INSERT INTO #OutputCategoryIdsDistinct ([CategoryId])
		SELECT DISTINCT CategoryId FROM #OutputCategoryIds
			LEFT OUTER JOIN Category C ON C.Id = CategoryId
		WHERE C.Published = 1 AND C.Deleted = 0

		SELECT @CategoriesByProductsOrKeywords = COALESCE(@CategoriesByProductsOrKeywords + ',' , '') 
			+ CAST(CategoryId as nvarchar(MAX)) FROM #OutputCategoryIdsDistinct

		DROP TABLE #OutputCategoryIds
		DROP TABLE #OutputCategoryIdsDistinct
	END

	DELETE #PageIndex FROM #PageIndex
		LEFT OUTER JOIN (SELECT MIN(IndexId) as RowId, ProductId FROM #PageIndex GROUP BY ProductId) AS KeepRows ON #PageIndex.IndexId = KeepRows.RowId
	WHERE KeepRows.RowId IS NULL
	SET @TotalRecords = @TotalRecords - @@rowcount

	CREATE TABLE #PageIndexDistinct
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndexDistinct ([ProductId]) SELECT [ProductId] FROM #PageIndex ORDER BY [IndexId]

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndexDistinct [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #ProductIdsBeforeFiltersApplied
	DROP TABLE #FilteredSpecificationAttributesToProduct
	DROP TABLE #FilteredSpecificationAttributes
	DROP TABLE #FilteredSpecificationAttributeOptions
	DROP TABLE #FilteredManufacturers
	DROP TABLE #KeywordProducts
	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #ProductsFilteredByCategories
	DROP TABLE #ProductsFilteredByPrices
	DROP TABLE #ProductsFilteredFinal
	DROP TABLE #PageIndex
	DROP TABLE #PageIndexDistinct
END

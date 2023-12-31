/****** Object: AutoTesting plugin sample data and configuration ******/

/* Hide address state province for checkout */
UPDATE [dbo].[Setting] SET Value = 'False' WHERE [Name] = 'addresssettings.stateprovinceenabled';

/* Enable auto testing robot */
IF NOT EXISTS (SELECT * FROM [dbo].[Setting] WHERE Name = 'autotestingsettings.enabledautotestingrobot')
	INSERT INTO [dbo].[Setting] (Name, Value, StoreId) VALUES ('autotestingsettings.enabledautotestingrobot', 'True', 0);
ELSE
    UPDATE [dbo].[Setting] SET Value = 'True' WHERE [Name] = 'autotestingsettings.enabledautotestingrobot';

/* Add auto testing plugin name to active widgets */
IF NOT EXISTS (SELECT * FROM [dbo].[Setting] WHERE Name = 'widgetsettings.activewidgetsystemnames' AND Value LIKE '%KSystem.Nop.Plugin.Misc.AutoTesting%')
BEGIN
    DECLARE @systemNames NVARCHAR(max)
    SET @systemNames = (SELECT TOP 1 Value FROM [dbo].[Setting] WHERE Name = 'widgetsettings.activewidgetsystemnames')
    SET @systemNames = (SELECT CONCAT (@systemNames, ',KSystem.Nop.Plugin.Misc.AutoTesting'))
    UPDATE [dbo].[Setting] SET Value = @systemNames WHERE [Name] = 'widgetsettings.activewidgetsystemnames'
END

 /* Samples of testing pages */
SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Pages] ON 

INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (1, N'Admin report - last executed task', NULL, N'ILastExecutedTaskUrlProvider', NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (2, N'Catalog - all manufacturers', N'/manufacturer/all', NULL, NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (3, N'Catalog - category', NULL, N'ICategoryUrlProvider', NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (4, N'Catalog - manufacturer', NULL, N'IManufacturerUrlProvider', NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (5, N'Home page', N'/', NULL, NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (6, N'Grouped product', NULL, N'IGroupedProductUrlProvider', NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (7, N'Checkout', N'/onepagecheckout', NULL, NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (8, N'Shopping cart', N'/cart', NULL, NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (9, N'Simple product', NULL, N'ISimpleProductUrlProvider', NULL)
INSERT [dbo].[KSY_AutoTesting_Pages] ([Id], [Name], [TestingUrl], [CustomUrlProvider], [ProviderParameters]) VALUES (10, N'Topic page - shipping & returns', N'/shipping-returns', NULL, NULL)

SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Pages] OFF
GO

/* All necessary commands for testing pages */
SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Commands] ON 

INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (1, 2, 10, N'.manufacturer-grid .item-box .manufacturer-item', NULL, NULL, N'Manufacturer box items exists', 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (2, 2, 400, NULL, NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (3, 3, 10, N'.product-grid .item-box .product-item', NULL, NULL, N'Product catalog items exists', 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (4, 3, 400, NULL, NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (5, 4, 10, N'.product-grid .item-box .product-item', NULL, NULL, N'Product catalog items exists', 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (6, 4, 400, NULL, NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (7, 5, 410, NULL, NULL, NULL, NULL, 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (8, 5, 500, N'AutoTesting/ClearShoppingCart', NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (9, 5, 400, NULL, NULL, NULL, NULL, 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (10, 5, 501, NULL, NULL, NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (11, 6, 20, N'button.add-to-cart-button:first', NULL, NULL, NULL, 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (12, 6, 500, N'addproducttocart/details/', NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (13, 6, 400, NULL, NULL, NULL, NULL, 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (14, 6, 501, NULL, NULL, NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (15, 7, 40, N'#billing-address-select', N'null', NULL, NULL, 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (16, 7, 30, N'#BillingNewAddress_FirstName', N'Jan', NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (17, 7, 30, N'#BillingNewAddress_LastName', N'Maskuliak', NULL, NULL, 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (18, 7, 30, N'#BillingNewAddress_Email', N'jan.maskuliak@gmail.com', NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (19, 7, 40, N'#BillingNewAddress_CountryId', N'60', NULL, NULL, 50)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (20, 7, 30, N'#BillingNewAddress_City', N'Brno', NULL, NULL, 60)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (21, 7, 30, N'#BillingNewAddress_Address1', N'Vlhká 159/2', NULL, NULL, 70)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (22, 7, 30, N'#BillingNewAddress_ZipPostalCode', N'60200', NULL, NULL, 80)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (23, 7, 30, N'#BillingNewAddress_PhoneNumber', N'+420606789456', NULL, NULL, 90)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (24, 7, 20, N'#checkout-step-billing .new-address-next-step-button', NULL, NULL, NULL, 100)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (25, 7, 500, N'checkout/OpcSaveBilling/', NULL, NULL, NULL, 110)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (26, 7, 20, N'#shippingoption_0', NULL, NULL, NULL, 130)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (27, 7, 11, N'input[name=shippingoption]', N'3', NULL, N'Shipping options exists', 120)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (28, 7, 20, N'.shipping-method-next-step-button', NULL, NULL, NULL, 140)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (29, 7, 501, NULL, NULL, NULL, NULL, 150)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (30, 7, 500, N'checkout/OpcSaveShippingMethod/', NULL, NULL, NULL, 160)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (31, 7, 11, N'input[name=paymentmethod]', N'2', NULL, N'Payment methods exists', 170)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (32, 7, 20, N'input[value="Payments.CheckMoneyOrder"]', NULL, NULL, NULL, 180)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (33, 7, 20, N'.payment-method-next-step-button', NULL, NULL, NULL, 190)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (34, 7, 501, NULL, NULL, NULL, NULL, 200)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (35, 7, 500, N'checkout/OpcSavePaymentMethod/', NULL, NULL, NULL, 210)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (36, 7, 20, N'.payment-info-next-step-button', NULL, NULL, NULL, 220)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (37, 7, 501, NULL, NULL, NULL, NULL, 230)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (38, 7, 500, N'checkout/OpcSavePaymentInfo/', NULL, NULL, NULL, 240)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (39, 7, 11, N'.totals .order-total', N'1', NULL, N'Order total element exist', 250)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (40, 7, 400, NULL, NULL, NULL, NULL, 260)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (41, 7, 501, NULL, NULL, NULL, NULL, 270)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (42, 8, 11, N'table.cart tbody tr', N'2', NULL, N'Shopping cart items exists', 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (43, 8, 11, N'#termsofservice', N'1', NULL, N'Terms of service exists', 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (44, 8, 11, N'#checkout', N'1', NULL, N'Checkout button exists', 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (45, 8, 20, N'#termsofservice', NULL, NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (46, 8, 401, NULL, NULL, NULL, NULL, 50)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (47, 8, 500, N'AutoTesting/SaveTestingPageReportMessages', NULL, NULL, NULL, 60)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (48, 8, 20, N'#checkout', NULL, NULL, NULL, 70)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (49, 8, 501, NULL, NULL, NULL, NULL, 80)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (50, 9, 20, N'button.add-to-cart-button', NULL, NULL, NULL, 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (51, 9, 500, N'addproducttocart/details/', NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (52, 9, 400, NULL, NULL, NULL, NULL, 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (53, 9, 501, NULL, NULL, NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (54, 10, 11, N'.page.topic-page .page-body', N'1', NULL, N'Topic page body element exists', 10)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (55, 10, 420, NULL, NULL, NULL, NULL, 20)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (56, 10, 500, N'AutoTesting/DeleteLastProfileAddress', NULL, NULL, NULL, 30)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (57, 10, 400, NULL, NULL, NULL, NULL, 40)
INSERT [dbo].[KSY_AutoTesting_Commands] ([Id], [PageId], [CommandTypeId], [Selector], [Value], [Name], [Message], [CommandOrder]) VALUES (58, 10, 501, NULL, NULL, NULL, NULL, 50)

SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Commands] OFF
GO

/* After deploy testing task */
SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Tasks] ON 
INSERT [dbo].[KSY_AutoTesting_Tasks] ([Id], [Name]) VALUES (1, N'After deploy')
SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_Tasks] OFF
GO

/* Sample pages mapped testing task */
SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ON 

INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (1, 2, 1, 40, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (2, 3, 1, 20, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (3, 1, 1, 100, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (4, 5, 1, 10, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (5, 4, 1, 30, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (6, 6, 1, 60, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (7, 9, 1, 50, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (8, 8, 1, 70, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (9, 7, 1, 80, 1)
INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] ([Id], [PageId], [TaskId], [PageOrder], [IncludedInTask]) VALUES (10, 10, 1, 90, 1)

SET IDENTITY_INSERT [dbo].[KSY_AutoTesting_TaskPage_Mapping] OFF
GO

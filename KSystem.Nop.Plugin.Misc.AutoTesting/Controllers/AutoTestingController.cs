namespace KSystem.Nop.Plugin.Misc.AutoTesting.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Orders;
    using global::Nop.Services.Common;
    using global::Nop.Services.Customers;
    using global::Nop.Services.Directory;
    using global::Nop.Services.Orders;
    using global::Nop.Services.Stores;
    using global::Nop.Web.Framework.Controllers;

    public partial class AutoTestingController : BasePluginController
    {
        private readonly IStoreContext _storeContext;

        private readonly IWorkContext _workContext;

        private readonly IAddressService _addressService;

        private readonly ICountryService _countryService;

        private readonly ICustomerService _customerService;

        private readonly IShoppingCartService _shoppingCartService;

        private readonly IStoreMappingService _storeMappingService;

        private readonly ITaskReportService _taskReportService;

        private readonly ITestingPageService _testingPageService;

        public AutoTestingController(
            IStoreContext storeContext,
            IWorkContext workContext,
            IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IStoreMappingService storeMappingService,
            ITaskReportService taskReportService,
            ITestingPageService testingPageService)
        {
            _storeContext = storeContext;
            _workContext = workContext;
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _storeMappingService = storeMappingService;
            _taskReportService = taskReportService;
            _testingPageService = testingPageService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> SaveTestingPageReportMessages(
            int pageId,
            IList<ReportMessageModel> reports)
        {
            var executedTask = await _taskReportService.GetLastExecutedTaskAsync();

            if (executedTask != null)
            {
                if (reports.Count > default(int))
                {
                    var testingPage = await _testingPageService.GetTestingPageByIdAsync(pageId);

                    foreach (var report in reports)
                    {
                        await _taskReportService.SaveReportedMessageEntryAsync(new ReportedMessage
                        {
                            ExecutedTaskId = executedTask.Id,
                            PageName = testingPage != null ? testingPage.Name : string.Empty,
                            Message = report.Message,
                            Success = report.Success
                        });
                    }
                }

                executedTask.LastFinish = DateTime.Now;
                await _taskReportService.SaveExecutedTaskEntryAsync(executedTask);
            }

            return new JsonResult(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ClearShoppingCart()
        {
            var cartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), 
                ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

            foreach (var cartItem in cartItems)
            {
                await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);
            }

            return new JsonResult(new { result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteLastProfileAddress()
        {
            var addresses = await (await _customerService.GetAddressesByCustomerIdAsync((await _workContext.GetCurrentCustomerAsync()).Id))
                .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
                .ToListAsync();

            if (addresses.Count() > default(int))
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var lastAddress = addresses.Last();

                if ((customer.BillingAddressId.HasValue && customer.BillingAddressId == lastAddress.Id)
                    || (customer.ShippingAddressId.HasValue && customer.ShippingAddressId == lastAddress.Id))
                {
                    if (addresses.Count > 1)
                    {
                        var firstAddress = addresses.First();
                        customer.BillingAddressId = firstAddress.Id;
                        customer.ShippingAddressId = firstAddress.Id;
                    }
                    else
                    {
                        customer.BillingAddressId = null;
                        customer.ShippingAddressId = null;
                    }

                    customer.IsTaxExempt = false;
                    customer.IsCompanyCustomer = false;
                    await _customerService.UpdateCustomerAsync(customer);
                }

                await _addressService.DeleteAddressAsync(lastAddress);
            }

            return new JsonResult(new { result = true });
        }
    }
}

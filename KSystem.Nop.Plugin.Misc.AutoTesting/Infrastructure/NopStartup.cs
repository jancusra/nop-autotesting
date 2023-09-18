namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Factories;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders;

    using global::Nop.Core.Infrastructure;

    public class NopStartup : INopStartup
    {
        public int Order => 1001;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAutoTestingPluginMenuBuilder, AutoTestingPluginMenuBuilder>();

            services.AddScoped<ISimpleProductUrlProvider, SimpleProductUrlProvider>();
            services.AddScoped<IGroupedProductUrlProvider, GroupedProductUrlProvider>();

            services.AddScoped<ICategoryUrlProvider, CategoryUrlProvider>();
            services.AddScoped<IManufacturerUrlProvider, ManufacturerUrlProvider>();

            services.AddScoped<ILastExecutedTaskUrlProvider, LastExecutedTaskUrlProvider>();

            services.AddScoped<IProductCustomService, ProductCustomService>();
            services.AddScoped<ITestingPageService, TestingPageService>();
            services.AddScoped<ITestingTaskService, TestingTaskService>();
            services.AddScoped<ITaskReportService, TaskReportService>();

            services.AddScoped<IAutoTestingFactory, AutoTestingFactory>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new AutoTestingViewLocationExpander());
            });
        }
    }
}

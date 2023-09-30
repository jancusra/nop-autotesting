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

    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1001;

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAutoTestingPluginMenuBuilder, AutoTestingPluginMenuBuilder>();

            services.AddScoped<ISimpleProductUrlProvider, SimpleProductUrlProvider>();
            services.AddScoped<IGroupedProductUrlProvider, GroupedProductUrlProvider>();

            services.AddScoped<ICategoryUrlProvider, CategoryUrlProvider>();
            services.AddScoped<IManufacturerUrlProvider, ManufacturerUrlProvider>();

            services.AddScoped<ILastExecutedTaskUrlProvider, LastExecutedTaskUrlProvider>();

            // services.AddScoped<IProductCustomService, ProductCustomService>();
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

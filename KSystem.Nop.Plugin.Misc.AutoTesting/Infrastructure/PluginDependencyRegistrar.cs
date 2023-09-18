namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Factories;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders;

    using global::Nop.Core.Configuration;
    using global::Nop.Core.Infrastructure;
    using global::Nop.Core.Infrastructure.DependencyManagement;

    public class PluginDependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 10100;

        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IAutoTestingPluginMenuBuilder, AutoTestingPluginMenuBuilder>();

            services.AddScoped<ISimpleProductUrlProvider, SimpleProductUrlProvider>();
            services.AddScoped<IGroupedProductUrlProvider, GroupedProductUrlProvider>();
            services.AddScoped<ISetProductUrlProvider, SetProductUrlProvider>();

            services.AddScoped<IInsuranceProductUrlProvider, InsuranceProductUrlProvider>();
            services.AddScoped<IOversizedProductUrlProvider, OversizedProductUrlProvider>();

            services.AddScoped<ICategoryUrlProvider, CategoryUrlProvider>();
            services.AddScoped<IManufacturerUrlProvider, ManufacturerUrlProvider>();

            services.AddScoped<ILastExecutedTaskUrlProvider, LastExecutedTaskUrlProvider>();

            services.AddScoped<ITestingPageService, TestingPageService>();
            services.AddScoped<ITestingTaskService, TestingTaskService>();
            services.AddScoped<ITaskReportService, TaskReportService>();

            services.AddScoped<IAutoTestingFactory, AutoTestingFactory>();
        }
    }
}
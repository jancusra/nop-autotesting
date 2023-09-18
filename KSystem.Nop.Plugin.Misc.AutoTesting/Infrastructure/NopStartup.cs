namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using global::Nop.Core.Infrastructure;

    public class NopStartup : INopStartup
    {
        public int Order => 1001;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new AutoTestingViewLocationExpander());
            });
        }
    }
}

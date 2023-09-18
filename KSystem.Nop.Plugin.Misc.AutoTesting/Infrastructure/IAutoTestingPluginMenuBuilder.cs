namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using System.Threading.Tasks;

    using global::Nop.Services.Plugins;
    using global::Nop.Web.Framework.Menu;

    public interface IAutoTestingPluginMenuBuilder
    {
        Task<SiteMapNode> BuildAsync(PluginDescriptor pluginDescriptor);
    }
}
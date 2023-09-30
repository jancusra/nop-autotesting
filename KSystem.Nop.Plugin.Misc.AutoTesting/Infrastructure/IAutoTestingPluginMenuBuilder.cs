namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using System.Threading.Tasks;

    using global::Nop.Services.Plugins;
    using global::Nop.Web.Framework.Menu;

    /// <summary>
    /// Represents administration menu builder for plugin
    /// </summary>
    public interface IAutoTestingPluginMenuBuilder
    {
        /// <summary>
        /// Build administration menu structure
        /// </summary>
        /// <param name="pluginDescriptor">NOP plugin descriptor with data about a plugin</param>
        /// <returns>Builded menu structure</returns>
        Task<SiteMapNode> BuildAsync(PluginDescriptor pluginDescriptor);
    }
}
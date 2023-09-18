namespace KSystem.Nop.Plugin.Misc.AutoTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Components;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure;

    using global::Nop.Core;
    using global::Nop.Services.Common;
    using global::Nop.Services.Cms;
    using global::Nop.Services.Plugins;
    using global::Nop.Web.Framework.Infrastructure;
    using global::Nop.Web.Framework.Menu;

    public class AutoTestingPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        private readonly IAutoTestingPluginMenuBuilder _autoTestingPluginMenuBuilder;

        public AutoTestingPlugin(
            IWebHelper webHelper,
            IAutoTestingPluginMenuBuilder autoTestingPluginMenuBuilder)
        {
            _webHelper = webHelper;
            _autoTestingPluginMenuBuilder = autoTestingPluginMenuBuilder;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/AutoTestingPlugin/Configure";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var ksystemRootNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "KSystem.Root");

            if (ksystemRootNode == null)
            {
                ksystemRootNode = new SiteMapNode();
                ksystemRootNode.SystemName = "KSystem.Root";
                ksystemRootNode.Title = "K-System";
                ksystemRootNode.Visible = true;
                ksystemRootNode.IconClass = "fas fa-dragon";

                rootNode.ChildNodes.Add(ksystemRootNode);
            }

            var pluginMenuNode = await _autoTestingPluginMenuBuilder.BuildAsync(PluginDescriptor);
            ksystemRootNode.ChildNodes.Add(pluginMenuNode);
        }

        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AutoTestingBodyEndViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> {
                PublicWidgetZones.BodyEndHtmlTagBefore
            });
        }

        public bool HideInWidgetList => false;
    }
}
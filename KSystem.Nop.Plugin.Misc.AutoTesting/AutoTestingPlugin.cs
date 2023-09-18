namespace KSystem.Nop.Plugin.Misc.AutoTesting
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using KSystem.Nop.Plugin.Core.Infrastructure;
    using KSystem.Nop.Plugin.Core.Plugins;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure;

    using global::Nop.Core;
    using global::Nop.Core.Infrastructure;
    using global::Nop.Services.Common;
    using global::Nop.Services.Cms;
    using global::Nop.Services.Localization;
    using global::Nop.Web.Framework.Infrastructure;
    using global::Nop.Web.Framework.Menu;

    public class AutoTestingPlugin : CoreBasePlugin, IMiscPlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        private readonly IAutoTestingPluginMenuBuilder _autoTestingPluginMenuBuilder;

        public AutoTestingPlugin(
            IWebHelper webHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            ICoreMenuBuilder coreMenuBuilder,
            IAutoTestingPluginMenuBuilder autoTestingPluginMenuBuilder)
            : base(languageService, localizationService, fileProvider, coreMenuBuilder)
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
            var ksytemRootNode = CoreMenuBuilder.GetKSystemRootNode(rootNode);
            var pluginMenuNode = await _autoTestingPluginMenuBuilder.BuildAsync(PluginDescriptor);
            ksytemRootNode.ChildNodes.Add(pluginMenuNode);
        }

        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AutoTestingBodyEnd";
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
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
    using global::Nop.Services.Localization;
    using global::Nop.Services.Plugins;
    using global::Nop.Web.Framework.Infrastructure;
    using global::Nop.Web.Framework.Menu;

    /// <summary>
    /// Represents autotesting plugin base definition
    /// </summary>
    public class AutoTestingPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        private readonly ILocalizationService _localizationService;

        private readonly IAutoTestingPluginMenuBuilder _autoTestingPluginMenuBuilder;

        public AutoTestingPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IAutoTestingPluginMenuBuilder autoTestingPluginMenuBuilder)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _autoTestingPluginMenuBuilder = autoTestingPluginMenuBuilder;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/AutoTestingPlugin/Configure";
        }

        /// <summary>
        /// Prepare administration menu items
        /// </summary>
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

        /// <summary>
        /// Install base items for plugin and prepare locale resources
        /// </summary>
        public override async Task InstallAsync()
        {
            await base.InstallAsync();

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["KSystem.Nop.Plugin.Misc.AutoTesting.AutoTesting"] = "Automated testing",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.TestingPages"] = "Testing pages",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.TestingTasks"] = "Testing tasks",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.TaskReports"] = "Task reports",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.TaskReportDetail"] = "Task report detail",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.RunTask"] = "Run task",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.LastRun"] = "Last runned",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.LastFinish"] = "Last finished",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.BackToList"] = "back to list",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Copy"] = "Copy",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.AutoTestingRobot"] = "Testing robot",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Views.TestingCommands"] = "Testing commands",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Views.AddNewCommand"] = "Add new command",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Views.UpdateCommand"] = "Update command",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Views.AddExistingPage"] = "Add existing page",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Views.UpdateTaskPage"] = "Update task page",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.EnabledAutoTestingRobot"] = "Enabled testing robot",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.TestingUrl"] = "Testing URL address",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CustomUrlProvider"] = "URL address provider interface",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.ProviderParameters"] = "Provider interface parameters",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.ProviderParametersWithFormat"] = "Provider interface parameters (query URL format)",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CommandType"] = "Testing command type",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Selector"] = "Element selector",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Value"] = "Expected value",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Name"] = "Parameter name",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Message"] = "Testing result message",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.CommandOrder"] = "Command order",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.PageName"] = "Page name",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.PageOrder"] = "Page order",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.IncludedInTask"] = "Included in task",
                ["KSystem.Nop.Plugin.Misc.AutoTesting.Fields.Success"] = "Successfully completed"
            });
        }

        /// <summary>
        /// Unistall base items for plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        /// <summary>
        /// Get base widget view component
        /// </summary>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AutoTestingBodyEndViewComponent);
        }

        /// <summary>
        /// Get list of allowed widget zones
        /// </summary>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> {
                PublicWidgetZones.BodyEndHtmlTagBefore
            });
        }

        /// <summary>
        /// Hide plugin in the list of widget zones
        /// </summary>
        public bool HideInWidgetList => false;
    }
}
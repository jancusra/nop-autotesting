namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Routing;

    using global::Nop.Services.Localization;
    using global::Nop.Services.Plugins;
    using global::Nop.Web.Framework.Menu;

    public class AutoTestingPluginMenuBuilder : IAutoTestingPluginMenuBuilder
    {
        private readonly ILocalizationService _localizationService;

        public AutoTestingPluginMenuBuilder(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        public async Task<SiteMapNode> BuildAsync(PluginDescriptor pluginDescriptor)
        {
            var pluginMenuItem = new SiteMapNode
            {
                Title =
                    await _localizationService.GetResourceAsync(
                        "KSystem.Nop.Plugin.Misc.AutoTesting.AutoTesting"),
                ControllerName = string.Empty,
                ActionName = string.Empty,
                IconClass = "fas fa-robot",
                Visible = true,
                RouteValues = null,
            };

            var configurationMenuItem = new SiteMapNode()
            {
                SystemName = "KSystem.AutoTesting.Configuration",
                Title =
                    await _localizationService.GetResourceAsync(
                        "Admin.Configuration.Settings"),
                ControllerName = "AutoTestingPlugin",
                ActionName = "Configure",
                IconClass = "fa fa-genderless",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
            };
            pluginMenuItem.ChildNodes.Add(configurationMenuItem);

            var testingPagesMenuItem = new SiteMapNode()
            {
                SystemName = "KSystem.AutoTesting.TestingPages",
                Title =
                    await _localizationService.GetResourceAsync(
                        "KSystem.Nop.Plugin.Misc.AutoTesting.TestingPages"),
                ControllerName = "TestingPages",
                ActionName = "List",
                IconClass = "fa fa-genderless",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
            };
            pluginMenuItem.ChildNodes.Add(testingPagesMenuItem);

            var testingTasksMenuItem = new SiteMapNode()
            {
                SystemName = "KSystem.AutoTesting.TestingTasks",
                Title =
                    await _localizationService.GetResourceAsync(
                        "KSystem.Nop.Plugin.Misc.AutoTesting.TestingTasks"),
                ControllerName = "TestingTasks",
                ActionName = "List",
                IconClass = "fa fa-genderless",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
            };
            pluginMenuItem.ChildNodes.Add(testingTasksMenuItem);

            var taskReportsMenuItem = new SiteMapNode()
            {
                SystemName = "KSystem.AutoTesting.TaskReports",
                Title =
                    await _localizationService.GetResourceAsync(
                        "KSystem.Nop.Plugin.Misc.AutoTesting.TaskReports"),
                ControllerName = "TaskReports",
                ActionName = "List",
                IconClass = "fa fa-genderless",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
            };
            pluginMenuItem.ChildNodes.Add(taskReportsMenuItem);

            return pluginMenuItem;
        }
    }
}
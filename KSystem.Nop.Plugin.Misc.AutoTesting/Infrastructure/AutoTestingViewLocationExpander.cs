namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Razor;

    public class AutoTestingViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //// {0} is the action, {1} is controller, {2} is area

            var controllers = new string[] { 
                "AutoTestingPlugin",
                "TaskReports",
                "TestingCommands",
                "TestingPages",
                "TestingTaskPages",
                "TestingTasks"
            };

            if (controllers.Contains(context.ControllerName))
            {
                viewLocations = new[]
                {
                    "/Plugins/KSystem.Nop.Plugin.Misc.AutoTesting/Views/{1}/{0}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[]
                {
                    "/Plugins/KSystem.Nop.Plugin.Misc.AutoTesting/Views/Shared/{0}.cshtml"
                }.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}

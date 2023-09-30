namespace KSystem.Nop.Plugin.Misc.AutoTesting.Components
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Factories;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;

    using global::Nop.Web.Framework.Components;
    using global::Nop.Web.Framework.Infrastructure;

    /// <summary>
    /// Represents view component to inject auto testing javascript code
    /// </summary>
    [ViewComponent(Name = "AutoTestingBodyEnd")]
    public class AutoTestingBodyEndViewComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAutoTestingFactory _autoTestingFactory;

        private readonly AutoTestingSettings _autoTestingSettings;

        public AutoTestingBodyEndViewComponent(
            IHttpContextAccessor httpContextAccessor,
            IAutoTestingFactory autoTestingFactory,
            AutoTestingSettings autoTestingSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _autoTestingFactory = autoTestingFactory;
            _autoTestingSettings = autoTestingSettings;
        }

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone)
        {
            if (!_autoTestingSettings.EnabledAutoTestingRobot || widgetZone != PublicWidgetZones.BodyEndHtmlTagBefore)
            {
                return Content(string.Empty);
            }

            TestingWidgetModel model = null;

            if (!string.IsNullOrEmpty(Request.Query[AutoTestingDefaults.TestingTaskPageUrlParameterName]))
            {
                var testingTaskPageMapId = Convert.ToInt32(Request.Query[AutoTestingDefaults.TestingTaskPageUrlParameterName][0]);
                model = await _autoTestingFactory.PrepareTestingWidgetModelAsync(testingTaskPageMapId);
            }
            else if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString(AutoTestingDefaults.NextTaskPageSessionKey))
                && Request.Path.HasValue)
            {
                var nextTaskPageMapId = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetString(AutoTestingDefaults.NextTaskPageSessionKey));
                model = await _autoTestingFactory.ValidateTaskPageAndPrepareTestingWidgetModelAsync(nextTaskPageMapId, Request.Path.Value);
            }

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Content(string.Empty);
            }
        }
    }
}

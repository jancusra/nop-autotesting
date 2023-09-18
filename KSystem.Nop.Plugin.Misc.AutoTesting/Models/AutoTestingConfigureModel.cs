namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models
{
    using global::Nop.Web.Framework.Models;
    using global::Nop.Web.Framework.Mvc.ModelBinding;

    public record AutoTestingConfigureModel : BaseNopModel, ISettingsModel
    {
        // Auto testing robot
        [NopResourceDisplayName("KSystem.Nop.Plugin.Misc.AutoTesting.Fields.EnabledAutoTestingRobot")]
        public bool EnabledAutoTestingRobot { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}
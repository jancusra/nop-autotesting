namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using global::Nop.Core.Configuration;

    public class AutoTestingSettings : ISettings
    {
        // Auto testing robot
        public bool EnabledAutoTestingRobot { get; set; }
    }
}

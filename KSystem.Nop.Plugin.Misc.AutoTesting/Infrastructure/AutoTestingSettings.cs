namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using global::Nop.Core.Configuration;

    /// <summary>
    /// Represents settings of auto testing plugin
    /// </summary>
    public class AutoTestingSettings : ISettings
    {
        /// <summary>
        /// Auto testing robot activation
        /// </summary>
        public bool EnabledAutoTestingRobot { get; set; }
    }
}

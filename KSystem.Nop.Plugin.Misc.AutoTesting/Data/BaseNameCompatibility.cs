namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using System;
    using System.Collections.Generic;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Mapping;

    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ExecutedTask), TableDefaults.ExecutedTaskTable },
            { typeof(ReportedMessage), TableDefaults.ReportedMessageTable },
            { typeof(TestingCommand), TableDefaults.TestingCommandTable },
            { typeof(TestingPage), TableDefaults.TestingPageTable },
            { typeof(TestingTask), TableDefaults.TestingTaskTable },
            { typeof(TestingTaskPageMap), TableDefaults.TestingTaskPageMapTable }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
            
        };
    }
}
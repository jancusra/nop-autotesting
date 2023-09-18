namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models
{
    using System.Collections.Generic;

    public record TestingSectionModel
    {
        public TestingSectionModel()
        {
            BaseCommand = new TestingCommandWidgetModel();
            TestingCommands = new List<TestingCommandWidgetModel>();
        }

        public TestingCommandWidgetModel BaseCommand { get; set; }

        public IList<TestingCommandWidgetModel> TestingCommands { get; set; }
    }
}

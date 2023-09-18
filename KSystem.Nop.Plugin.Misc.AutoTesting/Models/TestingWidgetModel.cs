namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models
{
    using System.Collections.Generic;

    public record TestingWidgetModel
    {
        public TestingWidgetModel()
        {
            BaseTestingCommands = new List<TestingCommandWidgetModel>();

            AjaxTestingCommands = new List<TestingSectionModel>();
            DOMNodeInsertedTestingCommands = new List<TestingSectionModel>();
        }

        public IList<TestingCommandWidgetModel> BaseTestingCommands { get; set; }

        public IList<TestingSectionModel> AjaxTestingCommands { get; set; }

        public IList<TestingSectionModel> DOMNodeInsertedTestingCommands { get; set; }
    }
}

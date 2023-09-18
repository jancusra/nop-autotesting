namespace KSystem.Nop.Plugin.Misc.AutoTesting.Models
{
    using KSystem.Nop.Plugin.Misc.AutoTesting.Enums;

    using global::Nop.Web.Framework.Models;

    public record TestingCommandWidgetModel : BaseNopEntityModel
    {
        public int PageId { get; set; }

        public CommandType CommandType { get; set; }

        public string Selector { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }
    }
}

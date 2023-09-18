namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    public class TestingCommand : BaseEntity
    {
        public int PageId { get; set; }

        public int CommandTypeId { get; set; }

        public string Selector { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public int CommandOrder { get; set; }
    }
}

namespace KSystem.Nop.Plugin.Misc.AutoTesting.Domain
{
    using global::Nop.Core;

    public class TestingTaskPageMap : BaseEntity
    {
        public int TaskId { get; set; }

        public int PageId { get; set; }

        public int PageOrder { get; set; }

        public bool IncludedInTask { get; set; }
    }
}

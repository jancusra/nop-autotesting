namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Mapping.Builders;

    /// <summary>
    /// Represents a Testing command entity builder
    /// </summary>
    public class TestingCommandBuilder : NopEntityBuilder<TestingCommand>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TestingCommand.PageId)).AsInt32().ForeignKey<TestingPage>();
        }
    }
}
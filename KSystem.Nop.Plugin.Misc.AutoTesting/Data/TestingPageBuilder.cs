namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Mapping.Builders;

    public class AutoPageCommandBuilder : NopEntityBuilder<TestingPage>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            
        }
    }
}
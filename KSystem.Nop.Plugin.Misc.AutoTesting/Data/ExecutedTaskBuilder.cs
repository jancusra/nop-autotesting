namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Mapping.Builders;

    public class ExecutedTaskBuilder : NopEntityBuilder<ExecutedTask>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ExecutedTask.TaskId)).AsInt32().ForeignKey<TestingTask>();
        }
    }
}
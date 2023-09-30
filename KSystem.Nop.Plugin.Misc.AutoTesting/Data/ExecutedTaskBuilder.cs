namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Mapping.Builders;

    /// <summary>
    /// Represents a Executed task entity builder
    /// </summary>
    public class ExecutedTaskBuilder : NopEntityBuilder<ExecutedTask>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ExecutedTask.TaskId)).AsInt32().ForeignKey<TestingTask>();
        }
    }
}
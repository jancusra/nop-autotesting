namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Mapping.Builders;

    /// <summary>
    /// Represents a Reported message entity builder
    /// </summary>
    public class ReportedMessageBuilder : NopEntityBuilder<ReportedMessage>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReportedMessage.ExecutedTaskId)).AsInt32().ForeignKey<ExecutedTask>();
        }
    }
}
﻿namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator.Builders.Create.Table;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Mapping.Builders;

    public class ReportedMessageBuilder : NopEntityBuilder<ReportedMessage>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReportedMessage.ExecutedTaskId)).AsInt32().ForeignKey<ExecutedTask>();
        }
    }
}
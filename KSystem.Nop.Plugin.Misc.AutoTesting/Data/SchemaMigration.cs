namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Extensions;
    using global::Nop.Data.Migrations;

    [NopMigration("2023/09/18 23:00:00:0000000", "Misc.AutoTesting base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            if (!Schema.Table(TableDefaults.TestingPageTable).Exists())
                Create.TableFor<TestingPage>();

            if (!Schema.Table(TableDefaults.TestingCommandTable).Exists())
                Create.TableFor<TestingCommand>();

            if (!Schema.Table(TableDefaults.TestingTaskTable).Exists())
                Create.TableFor<TestingTask>();

            if (!Schema.Table(TableDefaults.TestingTaskPageMapTable).Exists())
                Create.TableFor<TestingTaskPageMap>();

            if (!Schema.Table(TableDefaults.ExecutedTaskTable).Exists())
                Create.TableFor<ExecutedTask>();

            if (!Schema.Table(TableDefaults.ReportedMessageTable).Exists())
                Create.TableFor<ReportedMessage>();
        }
    }
}
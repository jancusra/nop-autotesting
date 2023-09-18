namespace KSystem.Nop.Plugin.Misc.AutoTesting.Data
{
    using FluentMigrator;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;

    using global::Nop.Data.Migrations;

    [SkipMigrationOnUpdate]
    [NopMigration("2022/08/10 15:00:00", "Misc.AutoTesting base schema")]
    public class SchemaMigration : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table(TableDefaults.TestingPageTable).Exists())
                _migrationManager.BuildTable<TestingPage>(Create);

            if (!Schema.Table(TableDefaults.TestingCommandTable).Exists())
                _migrationManager.BuildTable<TestingCommand>(Create);

            if (!Schema.Table(TableDefaults.TestingTaskTable).Exists())
                _migrationManager.BuildTable<TestingTask>(Create);

            if (!Schema.Table(TableDefaults.TestingTaskPageMapTable).Exists())
                _migrationManager.BuildTable<TestingTaskPageMap>(Create);

            if (!Schema.Table(TableDefaults.ExecutedTaskTable).Exists())
                _migrationManager.BuildTable<ExecutedTask>(Create);

            if (!Schema.Table(TableDefaults.ReportedMessageTable).Exists())
                _migrationManager.BuildTable<ReportedMessage>(Create);
        }

        public override void Down()
        {
        }
    }
}
using Umbraco.Cms.Infrastructure.Migrations;


namespace FlexiForms.Data.Tables
{
    public class AddFlexiFormsResponsesTable : MigrationBase
    {
        public AddFlexiFormsResponsesTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "AddFlexiFormsResponseTable");

            if (TableExists(FlexiFormConstants.Database.ResponseTable) == false)
            {
                Create.Table<FlexiFormResponsesSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", FlexiFormConstants.Database.ResponseTable);
            }
        }
    }
}

using NPoco;
using System.Data;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;


namespace FlexiForms.Data.Tables
{
    [TableName(FlexiFormConstants.Database.ResponseTable)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class FlexiFormResponsesSchema : IFlexiFormsSchema
    {
        public FlexiFormResponsesSchema()
        {
        }

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Form Identifier")]
        public required string FormIdentifier { get; set; }

        [Column("Name")]
        public required string Name { get; set; }

        [Column("Email")]
        public required string Email { get; set; }

        [Column("Fields")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? Fields { get; set; }

        [Column("InternalSent")]
        public required bool InternalSent { get; set; }

        [Column("ExternalSent")]
        public required bool ExternalSent { get; set; }

        [Column("CreateDate")]
        public required DateTime CreateDate { get; set; }
    }
}
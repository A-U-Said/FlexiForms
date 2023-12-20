using FlexiForms.Data.Tables;
using Newtonsoft.Json;
using NPoco.fastJSON;
using StackExchange.Profiling.Internal;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FlexiForms.Messages.Views
{
    public class ResponseView: IFlexiFormsView<FlexiFormResponsesSchema, ResponseView>
    {
        public static ResponseView CreateView(FlexiFormResponsesSchema dbSchema)
        {
            return new ResponseView()
            {
                Id = dbSchema.Id,
                FormIdentifier = dbSchema.FormIdentifier,
                Name = dbSchema.Name,
                Email = dbSchema.Email,
                Fields = dbSchema.Fields.HasValue()
                    ? JsonConvert.DeserializeObject<Dictionary<string, string>>(dbSchema.Fields)
                    : null,
                InternalSent = dbSchema.InternalSent,
                ExternalSent = dbSchema.ExternalSent,
                CreateDate = dbSchema.CreateDate
            };
        }

        public int Id { get; set; }
        public string FormIdentifier { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string>? Fields { get; set; }
        public bool InternalSent { get; set; }
        public bool ExternalSent { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

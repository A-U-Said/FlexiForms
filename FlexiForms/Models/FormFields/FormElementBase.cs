namespace FlexiForms.Models.FormFields
{
    public class FormElementBase : IFormElementBase
    {
        public Guid Id { get; set; }
        public string? Label { get; set; }
        public bool? IsMandatory { get; set; }
        public string? Value { get; set; }
    }
}

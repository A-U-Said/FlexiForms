namespace FlexForms.Models.FormFields
{
    public class FormElementBase : IFormElementBase
    {
        public string Label { get; set; }
        public bool IsMandatory { get; set; }
        public string Value { get; set; }
    }
}

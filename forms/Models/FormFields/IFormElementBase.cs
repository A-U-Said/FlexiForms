
namespace FlexForms.Models.FormFields
{
    public interface IFormElementBase
    {
        string Label { get; set; }
        bool IsMandatory { get; set; }
        string Value { get; set; }
    }

}

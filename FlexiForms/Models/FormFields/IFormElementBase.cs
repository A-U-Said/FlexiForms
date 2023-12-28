
namespace FlexiForms.Models.FormFields
{
    public interface IFormElementBase
    {
        Guid Id { get; set; }
        string? Label { get; set; }
        bool? IsMandatory { get; set; }
        string? Value { get; set; }
    }

}

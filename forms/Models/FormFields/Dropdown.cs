
namespace FlexForms.Models.FormFields
{
    public class Dropdown : FormElementBase
    {
        public IEnumerable<string> DropdownOptions { get; set; } = new List<string>();
    }
}

namespace FlexiForms.Models.FormFields
{
    public class RadioSection : FormElementBase
    {
        public IEnumerable<string> RadioOptions { get; set; } = new List<string>();
    }
}

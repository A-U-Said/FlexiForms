using System.ComponentModel.DataAnnotations;
using FlexForms.Models.FormFields;
using Umbraco.Cms.Core.Strings;

namespace FlexForms.Models
{
    public class ContactFormViewModel
    {
        public ContactFormViewModel()
        {
        }

        public ContactFormViewModel(IEnumerable<FormElementBase>? elements, string formIdentifier, IHtmlEncodedString successMessage)
        {
            Elements = elements;
            FormIdentifier = formIdentifier;
            SuccessMessage = successMessage;
        }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "example@example.org")]
        public string Email { get; set; }

        public IEnumerable<FormElementBase>? Elements { get; set; }
        public string FormIdentifier { get; set; }
        public IHtmlEncodedString SuccessMessage { get; set; }
    }
}

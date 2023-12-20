using System.ComponentModel.DataAnnotations;
using FlexiForms.Backoffice;
using FlexiForms.Backoffice.PropertyEditors;
using FlexiForms.Extensions;
using FlexiForms.Models.FormFields;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;


namespace FlexiForms.Models
{
    public class ContactFormViewModel
    {
        public ContactFormViewModel()
        {
        }

        public ContactFormViewModel(ContactForm contactForm)
        {
            Elements = contactForm.CreateViewElements();
            FormIdentifier = contactForm.Key.ToString();
            FormHeader = contactForm.FormHeader ?? "Contact Us";
            SuccessMessage = contactForm.SuccessMessage ?? new HtmlEncodedString("<p>Your enquirey has been sent</p>");
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


        // Everything above needs to be sent to the controller.
        // Everything below is for the sake of the view, and I don't want to pass it as loosely typed viewdata.
        public string? FormHeader { get; set; }
        public IHtmlEncodedString? SuccessMessage { get; set; }
        public CaptchaCredentialsValue? Captcha { get; set;  }
    }
}

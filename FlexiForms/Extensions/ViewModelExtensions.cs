using FlexiForms.Models.FormFields;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Xml.Linq;
using Umbraco.Cms.Web.Common.PublishedModels;


namespace FlexiForms.Extensions
{
    public static class ViewModelExtensions
    {

        public static List<FormElementBase> CreateViewElements(this ContactForm contactForm)
        {
            var elements = new List<FormElementBase>();

            var formelements = contactForm.Elements?
                .Select(x => x.Content)
                .Cast<FormElement>();

            foreach (var formelement in formelements)
            {
                switch (formelement)
                {
                    case FormTextbox:
                        elements.Add(new Textbox
                        {
                            Id = formelement.Key,
                            Label = formelement.Label,
                            IsMandatory = formelement.IsMandatory
                        });
                        break;

                    case FormCheckbox:
                        elements.Add(new Checkbox
                        {
                            Id = formelement.Key,
                            Label = formelement.Label,
                            IsMandatory = formelement.IsMandatory
                        });
                        break;

                    case FormTextArea:
                        elements.Add(new TextArea
                        {
                            Id = formelement.Key,
                            Label = formelement.Label,
                            IsMandatory = formelement.IsMandatory
                        });
                        break;

                    case FormRadioSection radioSection:
                        elements.Add(new RadioSection
                        {
                            Id = formelement.Key,
                            Label = formelement.Label,
                            IsMandatory = formelement.IsMandatory,
                            RadioOptions = radioSection.GetOptions()
                                .Select(x => x.Option ?? "")
                        });
                        break;

                    case FormDropdown dropdown:
                        elements.Add(new Dropdown
                        {
                            Id = formelement.Key,
                            Label = formelement.Label,
                            IsMandatory = formelement.IsMandatory,
                            DropdownOptions = dropdown.GetOptions()
                                .Select(x => x.Option ?? "")
                        });
                        break;

                    default:
                        break;
                }
            }
            return elements;
        }


        public static IEnumerable<FormOptionsList> GetOptions(this FormDropdown formDropdown)
        {
            if (formDropdown.DropdownOptions == null)
            {
                return new List<FormOptionsList>();
            }

            return formDropdown.DropdownOptions.Select(x => x.Content).Cast<FormOptionsList>();
        }


        public static IEnumerable<FormOptionsList> GetOptions(this FormRadioSection formRadio)
        {
            if (formRadio.RadioOptions == null)
            {
                return new List<FormOptionsList>();
            }

            return formRadio.RadioOptions.Select(x => x.Content).Cast<FormOptionsList>();
        }


        public static FormElement? GetElementById (this ContactForm contactForm, Guid Id)
        {
            return contactForm.Elements?
                .Where(x => x.Content.Key == Id)
                .Select(x => x.Content)
                .Cast<FormElement>()
                .FirstOrDefault();
        }
    } 
}

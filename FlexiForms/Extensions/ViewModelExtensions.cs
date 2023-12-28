using FlexiForms.Models.FormFields;
using FlexiForms.Models.ViewModels;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using FlexiFormModels = FlexiForms.Models.ViewModels;

namespace FlexiForms.Extensions
{
    public static class ViewModelExtensions
    {
        public static List<FormElementBase> CreateViewElements(this FlexiFormModels.ContactForm contactForm)
        {
            var elements = new List<FormElementBase>();

            foreach (var formelement in contactForm.Elements)
            {
                switch (formelement.ContentType.Key)
                {
                    case var elementKey when (elementKey == FlexiFormConstants.Identifiers.TextboxId):
                        elements.Add(new Textbox
                        {
                            Id = formelement.Key,
                            Label = formelement.Value<string>("label"),
                            IsMandatory = formelement.Value<bool>("isMandatory")
                        });
                        break;

                    case var elementKey when (elementKey == FlexiFormConstants.Identifiers.CheckboxId):
                        elements.Add(new Checkbox
                        {
                            Id = formelement.Key,
                            Label = formelement.Value<string>("label"),
                            IsMandatory = formelement.Value<bool>("isMandatory")
                        });
                        break;

                    case var elementKey when (elementKey == FlexiFormConstants.Identifiers.TextAreaId):
                        elements.Add(new TextArea
                        {
                            Id = formelement.Key,
                            Label = formelement.Value<string>("label"),
                            IsMandatory = formelement.Value<bool>("isMandatory")
                        });
                        break;

                    case var elementKey when (elementKey == FlexiFormConstants.Identifiers.RadioId):
                        elements.Add(new RadioSection
                        {
                            Id = formelement.Key,
                            Label = formelement.Value<string>("label"),
                            IsMandatory = formelement.Value<bool>("isMandatory"),
                            RadioOptions = formelement.Value<BlockListModel>("radioOptions")?
                                .Select(x => x.Content.Value<string>("option") ?? "")
                                ?? new List<string>()
                        });
                        break;

                    case var elementKey when (elementKey == FlexiFormConstants.Identifiers.DropdownId):
                        elements.Add(new Dropdown
                        {
                            Id = formelement.Key,
                            Label = formelement.Value<string>("label"),
                            IsMandatory = formelement.Value<bool>("isMandatory"),
                            DropdownOptions = formelement.Value<BlockListModel>("dropdownOptions")?
                                .Select(x => x.Content.Value<string>("option") ?? "")
                                ?? new List<string>()
                        });
                        break;

                    default:
                        break;
                }
            }
            return elements;
        }


        public static FormElementBase? GetElementById (this ContactForm contactForm, Guid Id)
        {
            return contactForm.Elements?
                .Where(x => x.Key == Id)
                .ToFormElementBase()
                .FirstOrDefault();
        }

        public static IEnumerable<FormElementBase> ToFormElementBase(this IEnumerable<IPublishedElement> source)
        {
            return source.Select(x => new FormElementBase()
            {
                Id = x.Key,
                Label = x.Value<string>("label") ?? "",
                IsMandatory = x.Value<bool>("isMandatory"),
            });
        }

    } 
}

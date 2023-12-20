using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Sections;

namespace FlexiForms.Backoffice.Section
{
    public class FlexiFormsSectionComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Sections().InsertAfter<SettingsSection, FlexiFormsSection>();
        }
    }


    public class FlexiFormsSection : ISection
    {
        public string Alias => FlexiFormConstants.Backoffice.SectionAlias;
        public string Name => FlexiFormConstants.Backoffice.SectionName;
    }

}
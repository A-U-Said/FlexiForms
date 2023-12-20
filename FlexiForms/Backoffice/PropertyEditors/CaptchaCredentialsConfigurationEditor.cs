using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace FlexiForms.Backoffice.PropertyEditors
{
    public class CaptchaCredentialsConfigurationEditor : ConfigurationEditor<CaptchaCredentialsConfiguration>
    {
        public CaptchaCredentialsConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) 
            : base(ioHelper, editorConfigurationParser)
        {
        }
    }
}
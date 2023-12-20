using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace FlexiForms.Backoffice.PropertyEditors
{
    [DataEditor(
    alias: FlexiFormConstants.Backoffice.CaptchaAlias,
    name: "Captcha",
    view: "~/App_Plugins/flexiForms/CaptchaPropertyEditor/CaptchaCredentials.html",
    Group = "Common",
    Icon = "icon-list")]
    public class CaptchaCredentials : DataEditor
    {
        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public CaptchaCredentials(
            IDataValueEditorFactory dataValueEditorFactory, 
            IIOHelper ioHelper,
            IEditorConfigurationParser editorConfigurationParser
        )
        : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() 
            => new CaptchaCredentialsConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }

}
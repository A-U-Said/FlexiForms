using System.Runtime.Serialization;
using Umbraco.Cms.Core.PropertyEditors;


namespace FlexiForms.Backoffice.PropertyEditors
{
    public class CaptchaCredentialsConfiguration
    {
        [ConfigurationField(
            "captchaCredentials", 
            "Define Captcha Credentials",
            "~/App_Plugins/flexiForms/CaptchaPropertyEditor/CaptchaCredentials.prevalues.html", 
            Description = "Add Captcha Credentials"
        )]
        public CaptchaCredential[]? CaptchaCredentials { get; set; }


        [DataContract]
        public class CaptchaCredential
        {
            [DataMember(Name = "alias")]
            public string Alias { get; set; } = string.Empty;

            [DataMember(Name = "sitekey")]
            public string Sitekey { get; set; } = string.Empty;

            [DataMember(Name = "clientSecret")]
            public string ClientSecret { get; set; } = string.Empty;

        }
    }
}
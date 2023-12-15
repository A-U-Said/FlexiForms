using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Serialization;


namespace FlexiForms.Backoffice
{
    [JsonConverter(typeof(NoTypeConverterJsonConverter<CaptchaCredentialsValue>))]
    [TypeConverter(typeof(CaptchaCredentialsValueConverter))]
    public class CaptchaCredentialsValue
    {
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "sitekey")]
        public string Sitekey { get; set; } = string.Empty;

        [DataMember(Name = "clientSecret")]
        public string ClientSecret { get; set; } = string.Empty;
    }
}

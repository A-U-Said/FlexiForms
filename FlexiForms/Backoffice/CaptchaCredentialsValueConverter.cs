using Newtonsoft.Json;
using System.Xml;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.PublishedCache;

namespace FlexiForms.Backoffice
{
    public class CaptchaCredentialsValueConverter : IPropertyValueConverter
    {
        private readonly ILogger<ImageCropperValueConverter> _logger;

        public CaptchaCredentialsValueConverter(ILogger<ImageCropperValueConverter> logger)
        {
            _logger = logger;
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(FlexiFormConstants.Backoffice.CaptchaAlias);
        }

        public bool? IsValue(object value, PropertyValueLevel level)
        {
            switch (level)
            {
                case PropertyValueLevel.Source:
                    return value != null && (value is not string || !string.IsNullOrWhiteSpace((string)value));
                case PropertyValueLevel.Inter:
                    return null;
                case PropertyValueLevel.Object:
                    return null;
                default:
                    throw new NotSupportedException($"Invalid level: {level}.");
            }
        }

        public Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(CaptchaCredentialsValue);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Element;
        }

        public object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            if (source == null)
            {
                return null;
            }

            var sourceString = source.ToString()!;
            CaptchaCredentialsValue? value;

            try
            {
                value = JsonConvert.DeserializeObject<CaptchaCredentialsValue>(sourceString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not deserialize string '{JsonString}' into an captcha credentials value.", sourceString);
                value = null;
            }

            return value;
        }

        public virtual object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            return inter;
        }

        public virtual object? ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            var xmlDoc = new XmlDocument();
            XmlElement parentElement = xmlDoc.CreateElement("values");
            xmlDoc.AppendChild(parentElement);

            XmlElement valueElement = xmlDoc.CreateElement("value");
            valueElement.InnerText = inter?.ToString() ?? string.Empty;
            parentElement.AppendChild(valueElement);
            
            return xmlDoc.CreateNavigator();
        }

    }
}

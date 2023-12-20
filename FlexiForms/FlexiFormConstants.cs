using Umbraco.Cms.Core.Trees;

namespace FlexiForms
{
    public static partial class FlexiFormConstants
    {
        public static class Application
        {
            public const string PluginName = "flexiForms";
            public static Guid BlockGridContentId = new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60");
        }

        public static class Backoffice
        {
            public const string CaptchaAlias = "FlexiForms.Captcha";
            public const string SectionAlias = "flexiForms";
            public const string SectionName = "FlexiForms";
            public const string TreeAlias = "responses";
            public const string TreeName = "FlexiForms Responses";
            public const string TreeUrl = $"{SectionAlias}/{TreeAlias}";
        }

        public static class Database
        {
            public const string ResponseTable = "FlexiFormResponses";
        }

        public static class SupportedContent
        {
            public const string BlockGrid = "Umbraco.BlockGrid";
            public static IEnumerable<string> GetSuppportedContent() => new List<string>() { BlockGrid };
        }
    }
}

using Umbraco.Cms.Core.Trees;

namespace FlexiForms
{
    public static partial class FlexiFormConstants
    {
        public static class Application
        {
            public const string PluginName = "flexiForms";
        }

        public static class Identifiers
        {
            public static Guid BlockGridContentId = new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60");
            public static Guid TextboxId = new Guid("6a2e2864-4fa5-47b5-898e-9cfa1d3475ed");
            public static Guid CheckboxId = new Guid("3b2d22c2-c424-4c52-92df-abb2b5644b34");
            public static Guid DropdownId = new Guid("7523246f-165d-4688-b7fb-cfbc63b157a7");
            public static Guid RadioId = new Guid("5bd24ccb-9a7a-4852-88a2-0be847cf221f");
            public static Guid TextAreaId = new Guid("4cc1937a-e7f0-47de-bac1-7f2a20692949");
        }

        public static class Backoffice
        {
            public const string CaptchaAlias = "FlexiForms.Captcha";
            public const string SectionAlias = "flexiForms";
            public const string SectionName = "FlexiForms";
            public const string TreeAlias = "responses";
            public const string TreeName = "FlexiForms Responses";
            public const string TreeUrl = $"{SectionAlias}/{TreeAlias}";
            public const string TreeRootUrl = $"{TreeUrl}/welcome";
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

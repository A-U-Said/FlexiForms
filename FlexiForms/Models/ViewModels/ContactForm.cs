using FlexiForms.Backoffice.PropertyEditors;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;


namespace FlexiForms.Models.ViewModels
{
    public class ContactForm
    {
        private readonly IPublishedElement _content;
        public ContactForm(IPublishedElement content)
        {
            _content = content;
        }

        public Guid Key => _content.Key;
        public const string ModelTypeAlias = "contactForm";
        public const PublishedItemType ModelItemType = PublishedItemType.Content;

        public CaptchaCredentialsValue? Captcha => _content.Value<CaptchaCredentialsValue>("captcha");
        public IEnumerable<IPublishedElement>? Elements => _content.Value<BlockListModel>("elements")?.Select(x => x.Content);
        public string? FormHeader => _content.Value<string>("formHeader");
        public string? FormIdentifier => _content.Value<string>("formIdentifier");
        public IEnumerable<string>? InternalRecipients => _content.Value<IEnumerable<string>>("internalRecipients");
        public IEnumerable<MediaWithCrops>? RecieptEmailAttachments => _content.Value<IEnumerable<MediaWithCrops>>("recieptEmailAttachments");
        public bool SendExternalEmail => _content.Value<bool>("sendExternalEmail");
        public bool SendInternalEmail => _content.Value<bool>("sendInternalEmail");
        public bool StoreResponse => _content.Value<bool>("storeResponse");
        public IHtmlEncodedString? SuccessMessage => _content.Value<IHtmlEncodedString>("successMessage");
        public IHtmlEncodedString? UserRecieptEmail => _content.Value<IHtmlEncodedString>("userRecieptEmail");
    }
}
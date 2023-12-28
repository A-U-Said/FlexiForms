using Microsoft.AspNetCore.Mvc;
using FlexiForms.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Core.Configuration.Models;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Newtonsoft.Json;
using System.Text;
using Umbraco.Cms.Core.IO;
using System.Globalization;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Serialization;
using FlexiFormModels = FlexiForms.Models;
using FlexiForms.Models.FormFields;
using FlexiForms.Extensions;
using FlexiForms.Backoffice.PropertyEditors;
using FlexiForms.Data.Tables;
using FlexiForms.Data.Repositories;

namespace FlexiForms.Controllers
{
    public class ContactFormController : SurfaceController
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ContactFormController> _logger;
        private readonly HttpClient _captchaClient;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly IPublishedContentQuery _publishedContentQuery;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
        private readonly IDataTypeService _dataTypeService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly BlockGridPropertyValueConverter _blockGridPropertyValueConverter;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly BlockEditorConverter _blockEditorConverter;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IFlexiFormsResponsesRepository _responseRepository;
        private readonly string _senderAddress;
        private readonly TextInfo _textinfo = new CultureInfo("en-US", false).TextInfo;

        public ContactFormController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IOptions<GlobalSettings> globalSettings,
            IEmailSender emailSender,
            ILogger<ContactFormController> logger,
            HttpClient captchaClient,
            IContentService contentService,
            IMediaService mediaService,
            IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
            IContentTypeService contentTypeService,
            IMediaTypeService mediaTypeService,
            IDataTypeService dataTypeService,
            IPublishedContentQuery publishedContentQuery,
            IPublishedSnapshotAccessor publishedSnapshotAccessor,
            IPublishedModelFactory publishedModelFactory,
            IFlexiFormsResponsesRepository responseRepository,
            BlockGridPropertyValueConverter blockGridPropertyValueConverter,
            BlockEditorConverter blockEditorConverter,
            MediaFileManager mediaFileManager)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _logger = logger;
            _emailSender = emailSender;
            _captchaClient = captchaClient;
            _captchaClient.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
            _contentService = contentService;
            var global = globalSettings.Value;
            _senderAddress = global.Smtp.From;
            _mediaService = mediaService;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
            _contentTypeService = contentTypeService;
            _mediaTypeService = mediaTypeService;
            _dataTypeService = dataTypeService;
            _mediaFileManager = mediaFileManager;
            _publishedContentQuery = publishedContentQuery;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _publishedModelFactory = publishedModelFactory;
            _responseRepository = responseRepository;
            _blockEditorConverter = blockEditorConverter;
            _blockGridPropertyValueConverter = blockGridPropertyValueConverter;
            _umbracoContextAccessor = umbracoContextAccessor;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContactFormViewModel model)
        {
            TempData.Add("FormType", model.FormIdentifier);

            if (CurrentPage == null)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            var publishedPageModel = _publishedContentQuery.Content(CurrentPage.Id);
            if (publishedPageModel == null)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            var formDetails = GetCorrespondingForm(model, publishedPageModel);
            if (formDetails == null)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            if (!ModelState.IsValid)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            if (formDetails.Captcha != null && !Request.Form.ContainsKey("g-recaptcha-response"))
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            var captcha = Request.Form["g-recaptcha-response"].ToString();
            if (formDetails.Captcha != null && !await IsValid(captcha, formDetails.Captcha))
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            foreach (var formElement in formDetails.Elements?.ToFormElementBase())
            {
                if (formElement.IsMandatory.Value && model.Elements.FirstOrDefault(x => x.Id == formElement.Id)?.Value == null)
                {
                    ModelState.AddModelError("Required field is empty", $"{formElement.Label} cannot be empty");
                    TempData.Add("Success", false);
                    return CurrentUmbracoPage();
                }
            }

            if (!_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbraco))
                throw new Exception("Umbraco context currently unavailable.");

            bool externalSent = false;
            if (formDetails.SendExternalEmail)
            {
                externalSent = await SendExternalEmail(model, formDetails);
            }

            bool internalSent = false;
            if (formDetails.SendInternalEmail)
            {
                externalSent = await SendInternalEmail(model, formDetails);
            }

            if (formDetails.StoreResponse && !string.IsNullOrEmpty(formDetails.FormIdentifier))
            {
                var dboResponse = new FlexiFormResponsesSchema()
                {
                    FormIdentifier = formDetails.FormIdentifier,
                    Name = model.Name,
                    Email = model.Email,
                    Fields = FormResponseToString(model.Elements, formDetails),
                    InternalSent = internalSent,
                    ExternalSent = externalSent,
                    CreateDate = DateTime.Now,
                };

                await _responseRepository.Create(dboResponse);
            }

            TempData.Add("Success", true);

            return RedirectToCurrentUmbracoPage();
        }


        private string? FormResponseToString(IEnumerable<FormElementBase>? responses, FlexiFormModels.ViewModels.ContactForm contactForm)
        {
            if (responses == null)
            {
                return null;
            }
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            foreach (var response in responses)
            {
                fieldValues.Add(contactForm.GetElementById(response.Id)?.Label, response.Value);
            }

            return JsonConvert.SerializeObject(fieldValues);
        }


        private FlexiFormModels.ViewModels.ContactForm? GetCorrespondingForm(ContactFormViewModel model, IPublishedContent publishedPageContent)
        {
            foreach (var publishedProperty in publishedPageContent.Properties
                .Where(x => FlexiFormConstants.SupportedContent.GetSuppportedContent().Contains(x.PropertyType.EditorAlias)))
            {

                switch (publishedProperty.PropertyType.EditorAlias)
                {
                    case FlexiFormConstants.SupportedContent.BlockGrid:
                        var blockGridModel = publishedProperty.GetValue() as BlockGridModel;
                        var foundForm = blockGridModel
                            .Where(x => x.Content.ContentType.Key == FlexiFormConstants.Identifiers.BlockGridContentId)
                            .Select(x => new FlexiFormModels.ViewModels.ContactForm(x.Content))
                            .Where(x => x.Key.ToString() == model.FormIdentifier)
                            .FirstOrDefault();

                        if (foundForm != null)
                        {
                            return foundForm;
                        }
                        break;

                    default:
                        return null;
                }

            }
            return null;
        }


        private async Task<bool> SendInternalEmail(ContactFormViewModel model, FlexiFormModels.ViewModels.ContactForm contactForm)
        {
            try
            {
                if (_senderAddress == null)
                {
                    _logger.LogError("No sender address found");
                    return false;
                }

                StringBuilder emailBody = new StringBuilder();
                if (model.Elements != null && model.Elements.Any())
                {
                    foreach (var element in model.Elements)
                    {
                        switch (element)
                        {
                            case Checkbox:
                                emailBody.Append($"<p><strong>{contactForm.GetElementById(element.Id)?.Label}:</strong> {(element.Value == "on" ? "Yes" : "No")} </p>");
                                break;
                            default:
                                emailBody.Append($"<p><strong>{contactForm.GetElementById(element.Id)?.Label}:</strong> {element.Value} </p>");
                                break;
                        }
                    }
                }

                emailBody.Append($"<p><strong>You can respond to this user at {model.Email}</strong></p>");

                EmailMessage message = new EmailMessage(_senderAddress,
                                                        contactForm.InternalRecipients.ToArray(),
                                                        null,
                                                        null,
                                                        null,
                                                        $"{contactForm.FormHeader} enquiry from: {_textinfo.ToTitleCase(model.Name.ToLower())}",
                                                        emailBody.ToString(),
                                                        true,
                                                        null);

                await _emailSender.SendAsync(message, emailType: "Contact");

                _logger.LogInformation("Contact Form Submitted Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Submitting Contact Form");
                return false;
            }
        }


        private async Task<bool> SendExternalEmail(ContactFormViewModel model, FlexiFormModels.ViewModels.ContactForm contactForm)
        {
            try
            {
                if (_senderAddress == null || model.Email == null)
                {
                    _logger.LogError("No sender and/or recipient address found");
                    return false;
                }

                List<EmailMessageAttachment> recieptAttachments = new List<EmailMessageAttachment>();
                if (contactForm.RecieptEmailAttachments != null)
                {
                    foreach (var attachment in contactForm.RecieptEmailAttachments)
                    {
                        var mediaItem = _mediaService.GetById(attachment.Key);
                        Stream stream = _mediaFileManager.GetFile(mediaItem, out string mediaFilePath);
                        var extension = mediaItem.HasProperty("umbracoExtension") ? mediaItem.GetValue("umbracoExtension").ToString() : ".pdf";

                        recieptAttachments.Add(new EmailMessageAttachment(stream, $"{mediaItem.Name}.{extension}"));
                    }
                }

                StringBuilder emailBody = new StringBuilder();
                emailBody.Append($"<p>Dear {_textinfo.ToTitleCase(model.Name.ToLower())},</p>");
                emailBody.Append(contactForm.UserRecieptEmail);


                EmailMessage message = new EmailMessage(_senderAddress,
                                                        new[] { model.Email },
                                                        null,
                                                        null,
                                                        null,
                                                        $"{contactForm.FormHeader} enquiry",
                                                        emailBody.ToString(),
                                                        true,
                                                        recieptAttachments.Count() > 0 ? recieptAttachments : null);

                await _emailSender.SendAsync(message, emailType: "Contact");

                _logger.LogInformation("Contact Form Submitted Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error When Submitting Contact Form");
                return false;
            }
        }


        private async Task<bool> IsValid(string captcha, CaptchaCredentialsValue captchaCredentials)
        {
            try
            {
                var postTask = await _captchaClient.PostAsync($"?secret={captchaCredentials.ClientSecret}&response={captcha}", new StringContent(""));
                var result = await postTask.Content.ReadAsStringAsync();
                var recaptchaResponse =  JsonConvert.DeserializeObject<RecaptchaResponseModel>(result);

                return recaptchaResponse.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Validating Captcha");
                return false;
            }
        }
    }
}

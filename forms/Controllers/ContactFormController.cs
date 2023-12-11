using Microsoft.AspNetCore.Mvc;
using FlexForms.Models;
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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;
using Umbraco.Cms.Core.IO;
using System.Globalization;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Serialization;

namespace FlexForms.Controllers
{
    public class ContactFormController : SurfaceController
    {
        private readonly IConfiguration _configuration;
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
            IConfiguration configuration,
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
            BlockGridPropertyValueConverter blockGridPropertyValueConverter,
            BlockEditorConverter blockEditorConverter,
            MediaFileManager mediaFileManager)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
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
            _blockEditorConverter = blockEditorConverter;
            _blockGridPropertyValueConverter = blockGridPropertyValueConverter;
        }

        private IPublishedElement? ConvertToElement(BlockItemData data)
        {
            IPublishedContentCache? publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

            // Only convert element types - content types will cause an exception when PublishedModelFactory creates the model
            IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType(data.ContentTypeKey);
            if (publishedContentType == null || publishedContentType.IsElement == false)
            {
                return null;
            }

            Dictionary<string, object?> propertyValues = data.RawPropertyValues;

            // Get the UDI from the deserialized object. If this is empty, we can fallback to checking the 'key' if there is one
            Guid key = data.Udi is GuidUdi gudi ? gudi.Guid : Guid.Empty;
            if (key == Guid.Empty && propertyValues.TryGetValue("key", out var keyo))
            {
                Guid.TryParse(keyo!.ToString(), out key);
            }

            IPublishedElement element = new PublishedElement(publishedContentType, key, propertyValues, false, PropertyCacheLevel.Unknown, _publishedSnapshotAccessor);
            //element = _publishedModelFactory.CreateModel(element);

            return element;
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

            var pageModel = _contentService.GetById(CurrentPage.Id);
            if (pageModel == null)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            var pageProperties = new List<IProperty>();
            var propertyGroups = _contentTypeBaseServiceProvider
                .GetContentTypeOf(pageModel)?
                .CompositionPropertyGroups
                .OrderBy(x => x.SortOrder);

            if (propertyGroups != null)
            {
                foreach (IGrouping<string, PropertyGroup> groupsByAlias in propertyGroups.GroupBy(x => x.Alias))
                {
                    foreach (PropertyGroup group in groupsByAlias)
                    {
                        IEnumerable<IProperty> groupProperties = pageModel.GetPropertiesForGroup(group);
                        pageProperties.AddRange(groupProperties);
                    }
                }
            }

            //gets data about blockgrid config
            //var contactForm = _contentTypeService.Get("contactForm");
            //var blockGridDataTemp = _dataTypeService.GetDataType("Form Grid");
            //var splitLayoutBlock = ((BlockGridConfiguration)blockGridDataTemp.Configuration).Blocks
            //    .FirstOrDefault(x => x.ContentElementTypeKey == contactForm.Key);

            FormObject? formDetails = null;
            var pageGridValues = pageProperties
                .Where(x => x.PropertyType.PropertyEditorAlias == "Umbraco.BlockGrid")
                .SelectMany(x => x.Values);

            foreach (var publishedGridValue in pageGridValues.Select(x => x.PublishedValue))
            {
                //  Official way of doing it
                //  var blogGridConverter = new BlockGridEditorDataConverter(new JsonNetSerializer());
                //  blogGridConverter.TryDeserialize(gridValue.PublishedValue.ToString(), out BlockEditorData blockEditorData);
                var publishedGrid = JsonConvert.DeserializeObject<BlockValue>(publishedGridValue.ToString());

                // Establish that the grid contains a form
                var formInstances = publishedGrid.ContentData
                            .Where(x => x.ContentTypeKey == new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60"));

                foreach (var instance in formInstances)
                {
                    var fff = ConvertToElement(instance);

                    var hhh = pageProperties
                        .Where(x => x.PropertyType.PropertyEditorAlias == "Umbraco.BlockGrid")
                        .FirstOrDefault();

                    var fg = _publishedContentQuery.Content(pageModel.Id);
                    var ee = fg.GetProperty(hhh.Alias);

                    IPublishedContentCache? publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
                    IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType(pageModel.ContentTypeId);
                    IPublishedPropertyType? publishedPropertyType = publishedContentType?.GetPropertyType(hhh.Alias);

                    BlockGridModel? waa = (BlockGridModel?)_blockGridPropertyValueConverter.ConvertIntermediateToObject(
                        CurrentPage,
                        ee.PropertyType, 
                        PropertyCacheLevel.Unknown,
                        publishedGridValue, 
                        false);

                    var bbb = waa;
                }

                if (formInstances.Any())
                {
                    var xxx = publishedGrid.ContentData
                        .Where(x => x.ContentTypeKey == new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60"))
                        .Select(x => JsonConvert.DeserializeObject<FormObject>(JsonConvert.SerializeObject(x.RawPropertyValues)))
                        .Where(x => x.FormIdentifier == model.FormIdentifier)
                        .FirstOrDefault();
                }

                //var mainSection = JsonConvert.DeserializeObject<BlockValue>(gridValue.PublishedValue.ToString());
                //var foundForm = mainSection.ContentData
                //    .Where(x => x.ContentTypeKey == new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60"))
                //    .Select(x => JsonConvert.DeserializeObject<FormObject>(JsonConvert.SerializeObject(x.RawPropertyValues)))
                //    .Where(x => x.FormIdentifier == model.FormIdentifier)
                //    .FirstOrDefault();

                //if (foundForm != null)
                //{
                //    formDetails = foundForm;
                //    break;
                //}
            }


            if (!ModelState.IsValid)
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            if (!Request.Form.ContainsKey("g-recaptcha-response"))
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            var captcha = Request.Form["g-recaptcha-response"].ToString();
            if (!await IsValid(captcha))
            {
                TempData.Add("Success", false);
                return CurrentUmbracoPage();
            }

            foreach (var element in model.Elements)
            {
                if (element.IsMandatory && string.IsNullOrEmpty(element.Value))
                {
                    ModelState.AddModelError("Required field is empty", $"{element.Label} cannot be empty");
                    TempData.Add("Success", false);
                    return CurrentUmbracoPage();
                }
            }

            

            TempData.Add("Success", await SendEmail(model, pageModel));

            return RedirectToCurrentUmbracoPage();

        }


        private async Task<bool> SendEmail(ContactFormViewModel model, IContent pageModel)
        {
            FormObject? formDetails = null;
            var blockGrids = pageModel.GetPropertiesByEditor("Umbraco.BlockGrid");

            foreach (var blockGrid in blockGrids)
            {
                foreach (var gridValue in blockGrid.Values)
                {
                    var mainSection = JsonConvert.DeserializeObject<BlockValue>(gridValue.PublishedValue.ToString());
                    var foundForm = mainSection.ContentData
                        .Where(x => x.ContentTypeKey == new Guid("da28b7d3-955e-4712-9ea9-8f35376bff60"))
                        .Select(x => JsonConvert.DeserializeObject<FormObject>(JsonConvert.SerializeObject(x.RawPropertyValues)))
                        .Where(x => x.FormIdentifier == model.FormIdentifier)
                        .FirstOrDefault();

                    if (foundForm != null)
                    {
                        formDetails = foundForm;
                        break;
                    }
                }
            }

            if (formDetails == null)
            {
                _logger.LogError("Error Finding Form");
                return false;
            }

            string formType = !string.IsNullOrEmpty(model.FormIdentifier) ? model.FormIdentifier : "Default";
            var sanitisedFormName = Regex.Replace(formType, @"\s+", "");

            var successfulInternalSend = await SendInternalEmail(model, sanitisedFormName, formType);

            if (!string.IsNullOrEmpty(formDetails.UserRecieptEmail))
            {
                var successfulUserSend = await SendRecieptEmail(model, sanitisedFormName, formType, formDetails);
                //Don't fail form if someone gives wrong email or user email fails
            }

            return successfulInternalSend;
        }


        async Task<bool> SendInternalEmail(ContactFormViewModel model, string sanitisedFormName, string formType)
        {
            try
            {
                var recipientAddresses = _configuration.GetSection("LhtContacts:" + sanitisedFormName).Get<List<string>>();

                if (_senderAddress == null || recipientAddresses == null)
                {
                    _logger.LogError("No sender and/or recipient address found");
                    return false;
                }

                string recipientString = "";
                foreach (var recipient in recipientAddresses)
                {
                    recipientString += $"{recipient},";
                }

                StringBuilder emailBody = new StringBuilder();
                if (model.Elements != null && model.Elements.Any())
                {
                    //foreach (var element in model.Elements)
                    //{
                    //    if (element.Type == typeof(Checkbox))
                    //    {
                    //        emailBody.Append($"<p><strong>{element.Label}:</strong> {(element.Value == "on" ? "Yes" : "No")} </p>");
                    //    }
                    //    else
                    //    {
                    //        emailBody.Append($"<p><strong>{element.Label}:</strong> {element.Value} </p>");
                    //    }
                    //}
                }
                //emailBody.Append($"<p><br>{model.Message}</p> ");
                emailBody.Append($"<p><strong>You can respond to this user at {model.Email}</strong></p>");

                EmailMessage message = new EmailMessage(_senderAddress,
                                                        recipientAddresses.ToArray(),
                                                        null,
                                                        null,
                                                        null,
                                                        $"{formType} enquiry from: {_textinfo.ToTitleCase(model.Name.ToLower())}",
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


        async Task<bool> SendRecieptEmail(ContactFormViewModel model, string sanitisedFormName, string formType, FormObject FormDetails)
        {
            try
            {
                var recipientAddress = model.Email;

                if (_senderAddress == null || recipientAddress == null)
                {
                    _logger.LogError("No sender and/or recipient address found");
                    return false;
                }

                List<EmailMessageAttachment> recieptAttachments = new List<EmailMessageAttachment>();
                if (FormDetails.RecieptEmailAttachments != null)
                {
                    var attachments = JsonConvert.DeserializeObject<List<Attachments>>(FormDetails.RecieptEmailAttachments);
                    foreach (var attachment in attachments)
                    {
                        var mediaItem = _mediaService.GetById(attachment.MediaKey);
                        Stream stream = _mediaFileManager.GetFile(mediaItem, out string mediaFilePath);
                        var extension = mediaItem.HasProperty("umbracoExtension") ? mediaItem.GetValue("umbracoExtension").ToString() : ".pdf";

                        recieptAttachments.Add(new EmailMessageAttachment(stream, $"{mediaItem.Name}.{extension}"));
                    }
                }

                StringBuilder emailBody = new StringBuilder();
                emailBody.Append($"<p>Dear {_textinfo.ToTitleCase(model.Name.ToLower())},</p>");
                emailBody.Append(FormDetails.UserRecieptEmail);


                EmailMessage message = new EmailMessage(_senderAddress,
                                                        new[] { recipientAddress },
                                                        null,
                                                        null,
                                                        null,
                                                        $"{formType} enquiry",
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


        public async Task<bool> IsValid(string captcha)
        {
            try
            {
                var captchaSecret = _configuration.GetValue<string>("ReCaptcha:ClientSecret");
                var postTask = await _captchaClient.PostAsync($"?secret={captchaSecret}&response={captcha}", new StringContent(""));
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

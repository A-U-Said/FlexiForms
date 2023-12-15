using static Umbraco.Cms.Core.PropertyEditors.ListViewConfiguration;
using Umbraco.Cms.Infrastructure.PublishedCache.DataSource;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using System;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models;

namespace FlexiForms.Models
{
    public class FormObject
    {
        public string? FormHeader { get; set; }
        public string? FormIdentifier { get; set; }
        public string? SuccessMessage { get; set; }
        public string? Elements { get; set; }
        public string? UserRecieptEmail { get; set; }
        public string? RecieptEmailAttachments { get; set; }
        public string? InternalRecipients { get; set; }

    }

    public class Attachments
    {
        public Guid Key { get; set; }
        public Guid MediaKey { get; set; }
    }
}

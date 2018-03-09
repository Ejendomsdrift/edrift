using System.Collections.Generic;
using System.Linq;
using Infrastructure.Messaging;

namespace MailMessaging.Models
{
    public abstract class EmailMessageBase : IMessage
    {
        protected EmailMessageBase()
        {
            AttachmentFiles = Enumerable.Empty<EmailAttachment>();
        }

        public string ToEmail { get; set; }

        public string ToName { get; set; }

        public string BccEmails { get; set; }

        public string CcEmails { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string Subject { get; set; }

        public string MailTemplateName { get; set; }

        public bool IsBodyHtml { get; set; }

        public IEnumerable<EmailAttachment> AttachmentFiles { get; set; }

        public Dictionary<string, string> ExtraTokens => GetExtraTokens();

        protected virtual Dictionary<string, string> GetExtraTokens()
        {
            return new Dictionary<string, string>();
        }
    }
}
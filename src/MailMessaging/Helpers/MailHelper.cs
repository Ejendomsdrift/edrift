using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using MailMessaging.Models;

namespace MailMessaging.Helpers
{
    public static class MailHelper
    {
        public static MailMessage MapMailMessage(EmailMessageBase message)
        {
            var from = new MailAddress(message.FromEmail, message.FromName);
            var to = new MailAddress(message.ToEmail, message.ToName);

            var result = new MailMessage(from, to);

            var template = GetMailTemplate(message.MailTemplateName);

            result.CC.Add(message.CcEmails);
            result.Bcc.Add(message.BccEmails);
            result.Subject = message.Subject;
            result.Body = ReplaceTokens(template, message.ExtraTokens);
            result.SubjectEncoding = Encoding.UTF8;
            result.BodyEncoding = Encoding.UTF8;
            result.IsBodyHtml = message.IsBodyHtml;

            foreach (var attachment in message.AttachmentFiles)
            {
                if (!File.Exists(attachment.PathToContent))
                {
                    continue;
                }

                var attachmentStream = new FileStream(attachment.PathToContent, FileMode.Open, FileAccess.Read);

                var mailAttacment = new Attachment(attachmentStream, attachment.Name);
                result.Attachments.Add(mailAttacment);
            }

            return result;
        }

        public static string GetMailTemplate(string mailTemplateName)
        {
            var result = string.Empty;

            //TODO: Get template by name (use Mongo to store templates)
            result = "We need implement mail template storage";

            return result;
        }

        public static string ReplaceTokens(string template, Dictionary<string,string> mailTokens)
        {
            foreach (var token in mailTokens)
            {
                template = template.Replace(token.Key, token.Value);
            }

            return template;
        }
    }
}

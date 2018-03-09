using System.Net.Mail;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using MailMessaging.Helpers;
using MailMessaging.Models;

namespace MailMessaging.Implementation
{
    public class MailSender: IHandler<EmailMessageBase>
    {
        public Task Handle(EmailMessageBase message)
        {
            var mailMessage = MailHelper.MapMailMessage(message);

            using (SmtpClient client = new SmtpClient())
            {
                return client.SendMailAsync(mailMessage);
            }
        }
    }
}

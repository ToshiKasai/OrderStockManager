using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OrderStockManager.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendMailAsync(message);
        }

        private Task configSendMailAsync(IdentityMessage message)
        {
            using (var msg = new MailMessage())
            using (var sc = new SmtpClient("ms37.kagoya.net", int.Parse("587")))
            {
                msg.From = new MailAddress("serverlog@j-fla.com", "system manager");
                msg.ReplyToList.Add(message.Destination);
                msg.To.Add(message.Destination);
                msg.Subject = message.Subject;
                msg.IsBodyHtml = false;
                msg.Body = message.Body.Replace(@"\r\n", Environment.NewLine);

                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailService:Account"], ConfigurationManager.AppSettings["emailService:Password"]);
                sc.EnableSsl = true;
                sc.Send(msg);
            }
            return Task.FromResult(0);
        }
    }
}
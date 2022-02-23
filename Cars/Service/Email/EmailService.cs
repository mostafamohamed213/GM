using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cars.Service.Email
{
    public class EmailService : IEmailService
    {
        private readonly string smtpHost, smtpUsername, smtpPassword, smtpFrom, toollink;
        private readonly int smtpPort;
        private readonly bool smtpEnableSSL;
        public EmailService(IConfiguration configuration)
        {
            toollink = configuration.GetSection("Core").GetSection("Tool_Link").Value;
            smtpHost = configuration.GetSection("Core").GetSection("SMTP_Host").Value;
            smtpUsername = configuration.GetSection("Core").GetSection("SMTP_Username").Value;
            smtpPassword = configuration.GetSection("Core").GetSection("SMTP_Password").Value;
            smtpFrom = configuration.GetSection("Core").GetSection("SMTP_From").Value;
            smtpPort = int.Parse(configuration.GetSection("Core").GetSection("SMTP_Port").Value);
            smtpEnableSSL = bool.Parse(configuration.GetSection("Core").GetSection("SMTP_EnableSSL").Value);
        }

        public void SendEmailForAddedOrderDetails(string orderPrefix, string to)
        {
            try
            {
                string mailbody = $"Dear ,<br/><br/> There is an order at your team is Added<br/> Order has ID : {orderPrefix} <br/> Please go and check it";
                SendMail(to, "Added Order", mailbody);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void SendEmailForDelayedOrderDetails(string orderPrefix, string teamName, string to)
        {
            try
            {
                string mailbody = $"Dear ,<br/><br/> There is an order at {teamName} is delayed and not passed to the next step yet <br/> Order has ID : {orderPrefix} <br/> Please go and check it";
                SendMail(to, $"Delayed Order at {teamName}", mailbody);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void SendEmailForRejectedOrderDetails(string orderPrefix, string teamName, string to)
        {
            try
            {
                string mailbody = $"Dear ,<br/><br/> There is an order at {teamName} is Rejected <br/> Order has ID : {orderPrefix}";
                SendMail(to, "Rejected Order at " + teamName, mailbody);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SendMail(string to, string subject, string emailBody)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to)) return;
                SmtpClient client = new SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = smtpEnableSSL;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = string.IsNullOrWhiteSpace(smtpUsername);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(smtpFrom);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = emailBody;
                client.SendAsync(mailMessage, null);
            }
            catch
            {
                return;
            }
        }
    }
}

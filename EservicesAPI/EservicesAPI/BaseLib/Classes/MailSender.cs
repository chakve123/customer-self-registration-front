using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using BaseLib.Common;

namespace BaseLib.Classes
{
    public class MailSender
    {
        public void SendMail(Smtp smtp)
        {
            var mm = new MailMessage
            {
                From = new MailAddress(smtp.FromMail),
                Subject = smtp.Subject,
                Body = smtp.Body,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };

            mm.To.Add(new MailAddress(smtp.ToMail));

            if (smtp.Attachment != null)
            {
                var ms = new MemoryStream(smtp.Attachment);
                var attachment = new Attachment(ms, smtp.AttachmentName, MediaTypeNames.Application.Octet);
                mm.Attachments.Add(attachment);
                // mm.Attachments.Add(new Attachment(ms, smtp.AttachmentName + ".pdf", "application/pdf"));
            }

            SmtpClient smptClient;

            if (!smtp.UseDefaultCredentials)
            {
                smptClient = new SmtpClient
                {
                    Host = smtp.SmtpUrl,
                    Port = smtp.Port,
                    EnableSsl = smtp.EnableSsl,
                    UseDefaultCredentials = smtp.UseDefaultCredentials,
                    Credentials = new NetworkCredential(smtp.UserName, smtp.Password)
                };
            }
            else
            {
                smptClient = new SmtpClient
                {
                    Host = smtp.SmtpUrl,
                    Port = smtp.Port,
                    EnableSsl = smtp.EnableSsl,
                    UseDefaultCredentials = smtp.UseDefaultCredentials
                };
            }

            smptClient.Send(mm);
        }
    }
}

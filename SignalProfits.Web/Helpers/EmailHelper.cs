using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SignalProfits.Web.Helpers
{
    public class EmailHelper
    {
        public static bool SendEmail(string body, string subject, string[] recipients, string[] attachments)
        {
            try
            {
                if (recipients == null || recipients.Count() == 0)
                    return false;

                using (MailMessage mail = new MailMessage())
                {
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    if (recipients != null)
                    {
                        foreach (string recipient in recipients)
                            mail.To.Add(new MailAddress(recipient));
                    }

                    mail.From = new MailAddress(AppConfiguration.EmailFrom);
                    mail.Subject = subject;
                    mail.Priority = MailPriority.Normal;
                    if (attachments != null)
                    {
                        foreach (string attachment in attachments)
                        {
                            mail.Attachments.Add(new Attachment(attachment));
                        }
                    }

                    using (SmtpClient smtp = new SmtpClient())
                    {                        
                        smtp.Send(mail);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
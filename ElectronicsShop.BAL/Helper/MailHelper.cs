using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ElectronicsShop.BAL.Helper
{
    public static class MailHelper
    {
        //General Config For Sending Mail
        public static bool SendMail(string to, string subject, string answer)
        {

            var from = "administratorEmail";
            MailMessage mail = new MailMessage(from, to);
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Host = "smtp.gmail.com",
                Credentials = new System.Net.NetworkCredential(from, "password"),
                EnableSsl = true
            };
            mail.Subject = subject;
            mail.Body = "" + answer + "";
            mail.IsBodyHtml = true;
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

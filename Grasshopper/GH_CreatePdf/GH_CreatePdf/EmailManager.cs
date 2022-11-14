using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace GH_CreatePdf
{
    public static class EmailManager
    {
        public static void SendEmail(string _fromAddress, string _password, string _toAddress, string _subject, string _message= "", string _attachmentPath = "", Stream _attachmentStream = null, string _attachmentName = "")
        {
            try
            {
                string senderName = "";
                MailAddress fromAddress;

                if (senderName == "")
                {
                    fromAddress = new MailAddress(_fromAddress);
                }
                else
                {
                    fromAddress = new MailAddress(_fromAddress, senderName);
                }

                MailAddress toAddress = new MailAddress(_toAddress);
                string fromPassword = _password;
                string subject = _subject;
                string body = _message;

                Attachment attachmentFile = null;
                Attachment attachmentStream = null;

                if (_attachmentPath != "" && File.Exists(_attachmentPath))
                {
                    attachmentFile = new Attachment(_attachmentPath);
                }
                if (_attachmentStream != null)
                {
                    attachmentStream = new Attachment(_attachmentStream, _attachmentName);
                }

                var smtp = new SmtpClient
                {
                    Host = "smtp-mail.outlook.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };

                var message = new MailMessage(fromAddress, toAddress);
                message.Subject = subject;
                message.Body = body;

                if (attachmentFile != null)
                {
                    message.Attachments.Add(attachmentFile);
                }
                if (attachmentStream != null)
                {
                    message.Attachments.Add(attachmentStream);
                }

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to send the mail. ErrorMessage: " + ex.ToString());
            }
        }
    }
}

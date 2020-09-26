using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class EmailManager
    {
        string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
        string password = ConfigurationManager.AppSettings["Password"];
        string recepient_TO = ConfigurationManager.AppSettings["Recipient_To"];
        string recepient_CC = ConfigurationManager.AppSettings["Recipient_CC"];
        string recepient_BCC = ConfigurationManager.AppSettings["Recipient_BCC"];
        public void SendEmail()
        {
			try
			{
                var from = new MailAddress(fromEmail);
                var to = new MailAddress(recepient_TO);

                var mail = new MailMessage(from, to);

                mail.CC.Add(recepient_CC);
                mail.Bcc.Add(recepient_BCC);
                mail.Subject = "Test mail";
                var htmlbody = "<html><head><style>" +
                                "table{border:1px solid red; }td{border:1px solid red;}" +
                                "</style></head><body>" +
                                "<table><tr><td> Name </td><td> Moni Kumari </td></tr></table></body>" +
                                "</html> ";
                mail.Body = "Hi, This is test mail" + "<br/>" + htmlbody;
                mail.IsBodyHtml = true;
                mail.Attachments.Add(new Attachment(@"D:\Temp Doc\TestZIP.zip"));
                mail.Attachments.Add(new Attachment(@"D:\Temp Doc\file.csv"));
                mail.Attachments.Add(new Attachment(@"D:\Temp Doc\Test.txt"));
                mail.Attachments.Add(new Attachment(@"D:\Temp Doc\wordfile.docx"));

                SmtpClient smtp = new SmtpClient
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, password),
                };
                smtp.Send(mail);
            }
			catch (Exception ex)
			{
				throw ex;
			}
        }
    }
}

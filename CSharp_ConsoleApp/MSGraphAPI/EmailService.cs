using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MSGraphAPI
{
    public class EmailService
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private string _token;
        private readonly string _fromEmailAddress;
        private GraphServiceClient _client;
        private string _recipient_TO;
        private string _recipient_CC;
        private string _recipient_BCC;

        public EmailService()
        {
            _tenantId = ConfigurationManager.AppSettings["TENANTID"];
            _clientId = ConfigurationManager.AppSettings["CLIENTID"];
            _clientSecret = ConfigurationManager.AppSettings["CLIENTSECRET"];
            _fromEmailAddress = ConfigurationManager.AppSettings["FROMEMAILADDRESS"];
            _recipient_TO = ConfigurationManager.AppSettings["RECEPIENT_TO"];
            _recipient_CC = ConfigurationManager.AppSettings["RECEPIENT_CC"];
            _recipient_BCC = ConfigurationManager.AppSettings["RECEPIENT_BCC"];
        }

        public async Task SendEmail()
        {
            try
            {
                await Connect();

                var message = new Message
                {
                    Subject = "Test Subject",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = "<p>Hi,</p><br/>This is test email send from Graph API<br /><p>Thank You.</p>"
                    },
                    ToRecipients = RecipientList(_recipient_TO),
                    CcRecipients = RecipientList(_recipient_CC),
                    BccRecipients = RecipientList(_recipient_BCC),
                    From = ToRecipient(_fromEmailAddress),
                    IsDraft = true,
                    Attachments = ToFileAttachmentList(@"D:\Temp Doc\Test.txt")
                };

                message = await _client.Users[_fromEmailAddress].Messages.Request().AddAsync(message);

                await _client.Users[_fromEmailAddress].Messages[message.Id]
                    .Send()
                    .Request()
                    .PostAsync();

                if(message.Id == null)
                {
                    Console.WriteLine("Error Occured! unable to send mail.");
                }
                else
                {
                    Console.WriteLine("Mail Sent Successfully.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static IMessageAttachmentsCollectionPage ToFileAttachmentList(string attachments)
        {
            var fileList = new MessageAttachmentsCollectionPage();
            foreach (string attachment in attachments.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                byte[] contentBytes = System.IO.File.ReadAllBytes(attachment);
                var attachmentName = attachment.Split('\\');

                string contentType = MimeMapping.GetMimeMapping(attachmentName[attachmentName.Length - 1]);

                var fileAttachment = new FileAttachment
                {
                    ODataType = "#microsoft.graph.fileAttachment",
                    Name = Path.GetFileName(attachment),
                    ContentLocation = attachment,
                    ContentBytes = contentBytes,
                    ContentType = contentType
                };

                fileList.Add(fileAttachment);
            }

            return fileList;
        }

        private static IList<Recipient> RecipientList(string address)
        {
            if (string.IsNullOrEmpty(address)) return new List<Recipient>();

            return address.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).Select(e => ToRecipient(e)).ToList();
        }

        private static Recipient ToRecipient(string emailAddress, string name = "")
        {
            return new Recipient
            {
                EmailAddress = ToEmailAddress(emailAddress, name)
            };
        }

        private static EmailAddress ToEmailAddress(string emailAddress, string name = "")
        {
            if (string.IsNullOrEmpty(emailAddress)) return null;

            return new EmailAddress
            {
                Address = emailAddress,
                Name = name
            };
        }

        #region Authentication

        private async Task<string> GetToken()
        {
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            var app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithClientSecret(_clientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .Build();

            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            _token = result.AccessToken;

            return _token;
        }

        public async Task Connect()
        {
            try
            {
                _client = new GraphServiceClient(
                                new DelegateAuthenticationProvider(
                                    async (requestMessage) =>
                                    {
                                        requestMessage.Headers.Add("Prefer", "outlook.body-content-type='text'");
                                        requestMessage.Headers.Authorization =
                                            new AuthenticationHeaderValue("Bearer", await GetToken());
                                    })
                    );

                //_token = await GetToken();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}

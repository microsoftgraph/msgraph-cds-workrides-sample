using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Mail
{
    public class MailService : IMailService
    {
        public async Task ComposeAndSendMailAsync(string subject,
                                                  string bodyContent,
                                                  BodyType contentType,
                                                  string recipients)
        {
            // Prepare the recipient list
            string[] splitter = { ";" };
            var splitRecipientsString = recipients.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            List<Recipient> recipientList = new List<Recipient>();

            foreach (string recipient in splitRecipientsString)
            {
                recipientList.Add(new Recipient { EmailAddress = new EmailAddress { Address = recipient.Trim() } });
            }

            var email = new Message
            {
                Body = new ItemBody
                {
                    Content = bodyContent,
                    ContentType = contentType,
                },
                Subject = subject,
                ToRecipients = recipientList,
            };

            await GraphClient.Instance.Beta.Me.SendMail(email, true).Request().PostAsync();
        }

        public async Task<IUserMessagesCollectionPage> GetCarpoolRequestsEmails(string riderId)
        {
            if (!string.IsNullOrEmpty(riderId))
            {
                return await GraphClient.Instance.Beta.Me.Messages
                    .Request()
                    .Filter($"startswith(subject,'May I ride with you') and from/emailAddress/address eq '{riderId}'")
                    .Select("subject,id")
                    .GetAsync();
            }

            return await GraphClient.Instance.Beta.Me.Messages
                    .Request()
                    .Filter($"startswith(subject,'May I ride with you')")
                    .Select("subject,id")
                    .GetAsync();
        } 
    }
}
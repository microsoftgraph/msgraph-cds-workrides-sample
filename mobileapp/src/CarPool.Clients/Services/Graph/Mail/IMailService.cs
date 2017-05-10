using Microsoft.Graph;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Mail
{
    public interface IMailService
    {
        Task ComposeAndSendMailAsync(string subject, string bodyContent, BodyType contentType, string recipients);

        Task<IUserMessagesCollectionPage> GetCarpoolRequestsEmails(string riderId);
    }
}
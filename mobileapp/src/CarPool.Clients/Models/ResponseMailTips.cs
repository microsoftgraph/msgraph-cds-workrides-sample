using Newtonsoft.Json;

namespace CarPool.Clients.Core.Models
{
    public class ResponseMailTips
    {
        public EmailAddress EmailAddress { get; set; }
        public AutomaticReplies AutomaticReplies { get; set; }
    }
}
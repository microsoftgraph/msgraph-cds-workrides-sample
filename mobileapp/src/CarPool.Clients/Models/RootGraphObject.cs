using Newtonsoft.Json;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Models
{
    public class RootGraphObject
    {
        [JsonProperty(PropertyName = "value")]
        public IList<ResponseMailTips> MailTips { get; set; }
    }
}
using Microsoft.Graph;

namespace CarPool.Clients.Core.Models
{
    public enum PopupType
    {
        Invitation,
        Carpool
    }

    public class PopupParameter
    {
        public PopupType PopupType { get; set; }
        public GraphUser User { get; set; }
    }
}
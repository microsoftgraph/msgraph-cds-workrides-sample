using Acr.UserDialogs;
using System.Threading.Tasks;

namespace Carpool.Clients.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public Task<bool> ShowConfirmAsync(string message, string title, string okLabel, string cancelLabel)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title, okLabel, cancelLabel);
        }
    }
}
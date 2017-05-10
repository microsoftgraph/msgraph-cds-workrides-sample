using System.Threading.Tasks;

namespace Carpool.Clients.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);

        Task<bool> ShowConfirmAsync(string message, string title, string okLabel, string cancelLabel);
    }
}
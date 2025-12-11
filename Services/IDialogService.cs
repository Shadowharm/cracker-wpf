using System.Windows;

namespace Cracker.Services
{
  public interface IDialogService
  {
    void ShowMessage(string message, string title = "Сообщение");
    void ShowError(string message, string title = "Ошибка");
    void ShowWarning(string message, string title = "Предупреждение");
    void ShowInfo(string message, string title = "Информация");
    bool ShowConfirmation(string message, string title = "Подтверждение");
    string ShowPasswordDialog();
    bool ShowLoginDialog(out string login, out string password);
  }
}


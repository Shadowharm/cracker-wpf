using System.Windows;

namespace Cracker.Services
{
  public class DialogService : IDialogService
  {
    public void ShowMessage(string message, string title = "Сообщение")
    {
      MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.None);
    }

    public void ShowError(string message, string title = "Ошибка")
    {
      MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowWarning(string message, string title = "Предупреждение")
    {
      MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void ShowInfo(string message, string title = "Информация")
    {
      MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string message, string title = "Подтверждение")
    {
      return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    public string ShowPasswordDialog()
    {
      var dialog = new dlgPassword();
      if (dialog.ShowDialog() == true)
      {
        return dialog.Password;
      }
      return null;
    }

    public bool ShowLoginDialog(out string login, out string password)
    {
      login = null;
      password = null;
      
      var dialog = new dlgLogin();
      if (dialog.ShowDialog() == true)
      {
        return true;
      }
      return false;
    }
  }
}


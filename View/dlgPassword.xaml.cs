using System.Windows;

namespace Cracker
{
  public partial class dlgPassword : Window
  {
    public string Password { get; private set; }

    public dlgPassword()
    {
      InitializeComponent();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      if (pbPass.Password != pbConfirm.Password)
      {
        MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      Password = pbPass.Password;
      DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
using System.ComponentModel;
using System.Windows;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class dlgLogin : Window
  {
    private LoginViewModel _viewModel;

    public dlgLogin()
    {
      InitializeComponent();
      this.Owner = App.Current.MainWindow;

      _viewModel = ServiceLocator.Instance.CreateLoginViewModel();
      DataContext = _viewModel;

      _viewModel.PropertyChanged += ViewModel_PropertyChanged;

      edLogin.Focus();
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(_viewModel.DialogResult))
      {
        if (_viewModel.DialogResult.HasValue)
        {
          DialogResult = _viewModel.DialogResult;
          Close();
        }
      }
    }

    private void ButtonDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Enter)
        LoginClick(null, new RoutedEventArgs());
      if (e.Key == System.Windows.Input.Key.Escape)
        _viewModel.ExitCommand.Execute(null);
    }

    private void LoginClick(object sender, RoutedEventArgs e)
    {
      _viewModel.Password = edPass.Password;
      _viewModel.LoginCommand.Execute(null);
    }
  }
}

using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class LoginViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;

    private string _login;
    private string _password;
    private bool? _dialogResult;

    public LoginViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      LoginCommand = new RelayCommand(PerformLogin);
      ExitCommand = new RelayCommand(Exit);
    }

    public string Login
    {
      get => _login;
      set => SetProperty(ref _login, value);
    }

    public string Password
    {
      get => _password;
      set => SetProperty(ref _password, value);
    }

    public bool? DialogResult
    {
      get => _dialogResult;
      set => SetProperty(ref _dialogResult, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand ExitCommand { get; }

    private void PerformLogin()
    {
      var user = _dataService.GetUserByCredentials(Login, Password);
      if (user == null)
      {
        _dialogService.ShowError("Имя пользователя или пароль не опознаны. Проверьте CAPS lock и язык ввода", "Отказ");
        return;
      }

      App.loginUser = user;
      DialogResult = true;
    }

    private void Exit()
    {
      DialogResult = false;
    }
  }
}


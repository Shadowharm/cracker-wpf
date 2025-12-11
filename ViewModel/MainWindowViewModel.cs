using System.Windows;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IDataService _dataService;

    private string _title = "Cracker scrum";
    private Visibility _backButtonVisibility = Visibility.Hidden;
    private Visibility _exitButtonVisibility = Visibility.Visible;

    public MainWindowViewModel(INavigationService navigationService, IDialogService dialogService, IDataService dataService)
    {
      _navigationService = navigationService;
      _dialogService = dialogService;
      _dataService = dataService;

      BackCommand = new RelayCommand(GoBack, () => _navigationService.CanGoBack);
      ExitCommand = new RelayCommand(Exit);
    }

    public string Title
    {
      get => _title;
      set => SetProperty(ref _title, value);
    }

    public Visibility BackButtonVisibility
    {
      get => _backButtonVisibility;
      set => SetProperty(ref _backButtonVisibility, value);
    }

    public Visibility ExitButtonVisibility
    {
      get => _exitButtonVisibility;
      set => SetProperty(ref _exitButtonVisibility, value);
    }

    public ICommand BackCommand { get; }
    public ICommand ExitCommand { get; }

    public void UpdateBackButtonVisibility()
    {
      BackButtonVisibility = _navigationService.CanGoBack ? Visibility.Visible : Visibility.Hidden;
    }

    private void GoBack()
    {
      _navigationService.GoBack();
    }

    private void Exit()
    {
      Application.Current.Shutdown();
    }

    public void Initialize()
    {
      try
      {
        if (_dataService.GetUsersCount() == 0)
        {
          _dialogService.ShowInfo("Первый запуск", "Вход в систему");
          _navigationService.NavigateTo(new pgUsers());
          return;
        }

        if (!_dialogService.ShowLoginDialog(out _, out _))
        {
          Application.Current.Shutdown();
          return;
        }

        _dialogService.ShowInfo($"Здравствуйте, {App.loginUser.name}", "Приветствие");

        if (App.isAdmin())
          _navigationService.NavigateTo(new pgUsers());
        else
          _navigationService.NavigateTo(new pgProjTask());
      }
      catch (System.Exception ex)
      {
        _dialogService.ShowError($"Ошибка подключения к БД: {ex.Message}", "Соединение");
        Application.Current.Shutdown();
      }
    }
  }
}


using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class UsersViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    
    private ObservableCollection<User> _users;
    private User _selectedUser;

    public UsersViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      AddCommand = new RelayCommand(AddUser);
      RemoveCommand = new RelayCommand(RemoveUser, () => SelectedUser != null);
      SetPasswordCommand = new RelayCommand<User>(SetPassword);

      Roles = new ObservableCollection<RoleItem>
      {
        new RoleItem { RoleDef = "Админ", Code = "Админ" },
        new RoleItem { RoleDef = "Менеджер", Code = "Менеджер" },
        new RoleItem { RoleDef = "Разработчик", Code = "Разработчик" }
      };
    }

    public ObservableCollection<User> Users
    {
      get => _users;
      set => SetProperty(ref _users, value);
    }

    public User SelectedUser
    {
      get => _selectedUser;
      set => SetProperty(ref _selectedUser, value);
    }

    public ObservableCollection<RoleItem> Roles { get; }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand SetPasswordCommand { get; }

    public void LoadData()
    {
      var users = _dataService.GetUsers();
      Users = new ObservableCollection<User>(users);
    }

    private void AddUser()
    {
      var user = new User
      {
        login = "Новый",
        passwd = "1",
        role = "Разработчик",
        name = "Новый пользователь системы"
      };

      _dataService.AddUser(user);

      try
      {
        _dataService.SaveChanges();
        LoadData();
        SelectedUser = Users.FirstOrDefault(u => u.id == user.id);
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }

    private void RemoveUser()
    {
      if (SelectedUser == null) return;

      if (!_dialogService.ShowConfirmation("Вы уверены?", "Удалить"))
        return;

      _dataService.DeleteUser(SelectedUser);

      try
      {
        _dataService.SaveChanges();
        LoadData();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }

    private void SetPassword(User user)
    {
      if (user == null) return;

      var password = _dialogService.ShowPasswordDialog();
      if (password != null)
      {
        user.passwd = password;
        try
        {
          _dataService.SaveChanges();
          _dialogService.ShowInfo("Пароль успешно установлен", "Пароль");
        }
        catch (Exception ex)
        {
          _dialogService.ShowError(ex.Message);
        }
      }
    }

    public void SaveChanges()
    {
      try
      {
        _dataService.SaveChanges();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }
  }

  public class RoleItem
  {
    public string RoleDef { get; set; }
    public string Code { get; set; }
  }
}


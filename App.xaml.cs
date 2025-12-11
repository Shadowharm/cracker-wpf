using System.Windows;
using System.Windows.Controls;
using Cracker.Services;

namespace Cracker
{
  public partial class App : Application
  {
    public static CrackerDB cDB = null;
    public static User loginUser = null;

    public static Frame MainFrame;

    public static bool isAdmin() => (App.loginUser != null && (App.loginUser.role.Equals("Админ")));
    public static bool isDev() => (App.loginUser != null && (App.loginUser.role.Equals("Разработчик")));
    public static bool isMan() => (App.loginUser != null && (App.loginUser.role.Equals("Менеджер")));

    public static INavigationService NavigationService => ServiceLocator.Instance.NavigationService;

    public static IDialogService DialogService => ServiceLocator.Instance.DialogService;

    public static IDataService DataService => ServiceLocator.Instance.DataService;
  }
}

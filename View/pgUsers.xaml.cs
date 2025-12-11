using System.Windows;
using System.Windows.Controls;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgUsers : Page
  {
    private UsersViewModel _viewModel;

    public pgUsers()
    {
      InitializeComponent();

      _viewModel = ServiceLocator.Instance.CreateUsersViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(Title);

      _viewModel.LoadData();
    }

    private bool locker = false;
    private void grdCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (locker) return;
      locker = true;
      ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, false);
      locker = false;
      _viewModel.SaveChanges();
    }
  }
}

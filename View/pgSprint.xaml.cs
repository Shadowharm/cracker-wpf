using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgSprint : Page
  {
    private SprintViewModel _viewModel;

    public pgSprint()
    {
      InitializeComponent();

      _viewModel = ServiceLocator.Instance.CreateSprintViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(Title);

      _viewModel.LoadData();
    }

    private void grdKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        e.Handled = true;
        grdSprint.CommitEdit();
      }
    }

    private bool locker = false;
    private void grdCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (e.EditAction == DataGridEditAction.Cancel) return;
      if (locker) return;
      locker = true;
      ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, false);
      locker = false;
      _viewModel.SaveChanges();
      _viewModel.LoadData();
    }
  }
}

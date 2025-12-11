using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgProjTask : Page
  {
    private ProjTaskViewModel _viewModel;

    public pgProjTask()
    {
      InitializeComponent();

      _viewModel = ServiceLocator.Instance.CreateProjTaskViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(Title);

      _viewModel.LoadData();
    }

    private void TaskClick(object sender, MouseButtonEventArgs e)
    {
      if (_viewModel.SelectedProject == null || _viewModel.SelectedTask == null) return;
      _viewModel.OpenTaskCommand.Execute(_viewModel.SelectedTask);
    }
  }
}

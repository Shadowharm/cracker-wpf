using System.Windows;
using System.Windows.Controls;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgTask : Page
  {
    private TaskViewModel _viewModel;
    private Task _task;
    private Project _project;

    public pgTask(Task task, Project project)
    {
      InitializeComponent();

      _task = task;
      _project = project;

      _viewModel = ServiceLocator.Instance.CreateTaskViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _viewModel.Initialize(_task, _project);

      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(_viewModel.PageTitle);
    }
  }
}

using System.Windows;
using System.Windows.Controls;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgReport : Page
  {
    private ReportViewModel _viewModel;
    private Project _project;

    public pgReport(Project project)
    {
      InitializeComponent();

      _project = project;
      _viewModel = ServiceLocator.Instance.CreateReportViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(_viewModel.PageTitle);

      _viewModel.Initialize(_project);
    }
  }
}


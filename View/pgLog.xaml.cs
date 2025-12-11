using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgLog : Page
  {
    private LogViewModel _viewModel;
    private Task _task;

    public pgLog(Task task)
    {
      InitializeComponent();
      _task = task;

      _viewModel = ServiceLocator.Instance.CreateLogViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _viewModel.Initialize(_task);

      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(_viewModel.PageTitle);

      Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
      {
        scrollViewer.ScrollToTop();
      }));
    }
  }
}

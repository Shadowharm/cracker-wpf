using System;
using System.Windows;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class MainWindow : Window
  {
    private MainWindowViewModel _viewModel;

    public MainWindow()
    {
      InitializeComponent();

      try
      {
        App.cDB = new CrackerDB();
        ServiceLocator.Instance.Initialize(App.cDB);
        ServiceLocator.Instance.NavigationService.MainFrame = MainFrame;
        App.MainFrame = MainFrame;

        _viewModel = ServiceLocator.Instance.CreateMainWindowViewModel();
        DataContext = _viewModel;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      _viewModel?.Initialize();
    }

    private void MainFrame_ContentRendered(object sender, EventArgs e)
    {
      _viewModel?.UpdateBackButtonVisibility();
    }

    public void UpdateTitle(string title)
    {
      if (_viewModel != null)
      {
        _viewModel.Title = title;
      }
    }
  }
}

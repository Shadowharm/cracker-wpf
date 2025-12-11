using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Cracker.Services;
using Cracker.ViewModel;

namespace Cracker
{
  public partial class pgNote : Page
  {
    private NoteViewModel _viewModel;
    private Task _task;

    public pgNote(Task task)
    {
      InitializeComponent();
      _task = task;

      _viewModel = ServiceLocator.Instance.CreateNoteViewModel();
      DataContext = _viewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _viewModel.Initialize(_task);

      var mainWindow = (MainWindow)Application.Current.MainWindow;
      mainWindow.UpdateTitle(_viewModel.PageTitle);

      txtNewComment.Focus();
      ScrollToEnd();
    }

    private void ScrollToEnd()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
      {
        scrollViewer.ScrollToEnd();
      }));
    }

    private void txtNewComment_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        if (Keyboard.Modifiers != ModifierKeys.Shift)
        {
          e.Handled = true;
          if (_viewModel.SendCommand.CanExecute(null))
          {
            _viewModel.SendCommand.Execute(null);
            ScrollToEnd();
          }
        }
      }
    }
  }
}

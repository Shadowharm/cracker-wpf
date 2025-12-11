using System.Windows.Controls;

namespace Cracker.Services
{
  public class NavigationService : INavigationService
  {
    private Frame _mainFrame;

    public Frame MainFrame
    {
      get => _mainFrame;
      set => _mainFrame = value;
    }

    public bool CanGoBack => _mainFrame?.CanGoBack ?? false;

    public void NavigateTo<TPage>() where TPage : Page, new()
    {
      _mainFrame?.Navigate(new TPage());
    }

    public void NavigateTo(Page page)
    {
      _mainFrame?.Navigate(page);
    }

    public void GoBack()
    {
      if (CanGoBack)
        _mainFrame?.GoBack();
    }
  }
}


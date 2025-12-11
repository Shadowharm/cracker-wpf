using System.Windows.Controls;

namespace Cracker.Services
{
  public interface INavigationService
  {
    void NavigateTo<TPage>() where TPage : Page, new();
    void NavigateTo(Page page);
    void GoBack();
    bool CanGoBack { get; }
  }
}


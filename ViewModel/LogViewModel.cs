using System.Collections.ObjectModel;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class LogViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;

    private Task _task;
    private ObservableCollection<Log> _logs;

    public LogViewModel(IDataService dataService)
    {
      _dataService = dataService;
    }

    public string PageTitle => $"Лог к {_task?.title}";

    public ObservableCollection<Log> Logs
    {
      get => _logs;
      set => SetProperty(ref _logs, value);
    }

    public void Initialize(Task task)
    {
      _task = task;
      LoadData();
    }

    public void LoadData()
    {
      if (_task == null) return;

      var logs = _dataService.GetLogsByTask(_task.id);
      Logs = new ObservableCollection<Log>(logs);
    }
  }
}


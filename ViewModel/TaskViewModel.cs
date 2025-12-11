using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class TaskViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private Task _task;
    private Project _project;
    private bool _isNewTask;

    private string _originalTitle;
    private string _originalNote;
    private string _originalStatus;
    private long? _originalDeveloperId;
    private long? _originalManagerId;
    private long? _originalSprintId;
    private int? _originalLeed;
    private bool _originalRush;

    private string _title;
    private string _note;
    private string _status;
    private User _manager;
    private User _developer;
    private Sprint _sprint;
    private int? _leed;
    private bool _rush;

    private ObservableCollection<User> _managers;
    private ObservableCollection<User> _developers;
    private ObservableCollection<Sprint> _sprints;
    private ObservableCollection<string> _statuses;

    private Visibility _sprintButtonVisibility = Visibility.Visible;
    private Visibility _noteButtonVisibility = Visibility.Visible;
    private Visibility _logButtonVisibility = Visibility.Visible;
    private Visibility _assignDeveloperButtonVisibility = Visibility.Collapsed;
    private bool _isTitleReadOnly;
    private bool _isManagerEnabled;
    private bool _isDeveloperEnabled;
    private bool _isSprintEnabled;
    private bool _isLeedReadOnly;
    private bool _isRushEnabled;
    private bool _isNoteReadOnly;

    public TaskViewModel(IDataService dataService, IDialogService dialogService, INavigationService navigationService)
    {
      _dataService = dataService;
      _dialogService = dialogService;
      _navigationService = navigationService;

      SaveCommand = new RelayCommand(Save);
      BackCommand = new RelayCommand(GoBack);
      SprintCommand = new RelayCommand(GoToSprints);
      NoteCommand = new RelayCommand(GoToNotes);
      LogCommand = new RelayCommand(GoToLogs);
      AssignDeveloperCommand = new RelayCommand(AssignDeveloper);

      InitializeStatuses();
    }

    #region Properties

    public string TaskTitle
    {
      get => _title;
      set => SetProperty(ref _title, value);
    }

    public string Note
    {
      get => _note;
      set => SetProperty(ref _note, value);
    }

    public string Status
    {
      get => _status;
      set => SetProperty(ref _status, value);
    }

    public User Manager
    {
      get => _manager;
      set => SetProperty(ref _manager, value);
    }

    public User Developer
    {
      get => _developer;
      set => SetProperty(ref _developer, value);
    }

    public Sprint Sprint
    {
      get => _sprint;
      set => SetProperty(ref _sprint, value);
    }

    public int? Leed
    {
      get => _leed;
      set => SetProperty(ref _leed, value);
    }

    public bool Rush
    {
      get => _rush;
      set => SetProperty(ref _rush, value);
    }

    public ObservableCollection<User> Managers
    {
      get => _managers;
      set => SetProperty(ref _managers, value);
    }

    public ObservableCollection<User> Developers
    {
      get => _developers;
      set => SetProperty(ref _developers, value);
    }

    public ObservableCollection<Sprint> Sprints
    {
      get => _sprints;
      set => SetProperty(ref _sprints, value);
    }

    public ObservableCollection<string> Statuses
    {
      get => _statuses;
      set => SetProperty(ref _statuses, value);
    }

    public string PageTitle => $"Задача {_task?.title} проекта {_project?.name}";

    public Visibility SprintButtonVisibility
    {
      get => _sprintButtonVisibility;
      set => SetProperty(ref _sprintButtonVisibility, value);
    }

    public Visibility NoteButtonVisibility
    {
      get => _noteButtonVisibility;
      set => SetProperty(ref _noteButtonVisibility, value);
    }

    public Visibility LogButtonVisibility
    {
      get => _logButtonVisibility;
      set => SetProperty(ref _logButtonVisibility, value);
    }

    public Visibility AssignDeveloperButtonVisibility
    {
      get => _assignDeveloperButtonVisibility;
      set => SetProperty(ref _assignDeveloperButtonVisibility, value);
    }

    public bool IsTitleReadOnly
    {
      get => _isTitleReadOnly;
      set => SetProperty(ref _isTitleReadOnly, value);
    }

    public bool IsManagerEnabled
    {
      get => _isManagerEnabled;
      set => SetProperty(ref _isManagerEnabled, value);
    }

    public bool IsDeveloperEnabled
    {
      get => _isDeveloperEnabled;
      set => SetProperty(ref _isDeveloperEnabled, value);
    }

    public bool IsSprintEnabled
    {
      get => _isSprintEnabled;
      set => SetProperty(ref _isSprintEnabled, value);
    }

    public bool IsLeedReadOnly
    {
      get => _isLeedReadOnly;
      set => SetProperty(ref _isLeedReadOnly, value);
    }

    public bool IsRushEnabled
    {
      get => _isRushEnabled;
      set => SetProperty(ref _isRushEnabled, value);
    }

    public bool IsNoteReadOnly
    {
      get => _isNoteReadOnly;
      set => SetProperty(ref _isNoteReadOnly, value);
    }

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand SprintCommand { get; }
    public ICommand NoteCommand { get; }
    public ICommand LogCommand { get; }
    public ICommand AssignDeveloperCommand { get; }

    #endregion

    public void Initialize(Task task, Project project)
    {
      _project = project;
      _isNewTask = task == null;

      LoadComboBoxData();

      if (_isNewTask)
      {
        _task = new Task
        {
          project_id = project.id,
          title = "Новая",
          status = "Новая",
          created = DateTime.Now,
          rush = false,
          leed = 24
        };

        if (App.loginUser.role == "Менеджер")
          _task.manager = App.loginUser;
        if (App.loginUser.role == "Разработчик")
          _task.developer = App.loginUser;

        NoteButtonVisibility = Visibility.Collapsed;
        LogButtonVisibility = Visibility.Collapsed;
      }
      else
      {
        _task = task;
        SaveOriginalValues();
      }

      TaskTitle = _task.title;
      Note = _task.note;
      Status = _task.status;
      Manager = _task.manager;
      Developer = _task.developer;
      Sprint = _task.sprint;
      Leed = _task.leed;
      Rush = _task.rush;

      ConfigureAccessRights();
      ConfigureAssignDeveloperButton();
    }

    private void ConfigureAssignDeveloperButton()
    {
      AssignDeveloperButtonVisibility = App.isMan() ? Visibility.Visible : Visibility.Collapsed;
    }

    private void InitializeStatuses()
    {
      Statuses = new ObservableCollection<string>
      {
        "Новая",
        "Готова к работе",
        "В работе",
        "На обсуждении",
        "Ревью",
        "Выливка",
        "Проверка",
        "Проверена",
        "Релиз",
        "Выполнена",
        "Отменена",
        "Возвращена",
        "Доработка"
      };
    }

    private void LoadComboBoxData()
    {
      Managers = new ObservableCollection<User>(_dataService.GetManagers());
      Developers = new ObservableCollection<User>(_dataService.GetDevelopers());
      Sprints = new ObservableCollection<Sprint>(_dataService.GetSprints());
    }

    private void SaveOriginalValues()
    {
      _originalTitle = _task.title;
      _originalNote = _task.note;
      _originalStatus = _task.status;
      _originalDeveloperId = _task.developer_id;
      _originalManagerId = _task.manager_id;
      _originalSprintId = _task.sprint_id;
      _originalLeed = _task.leed;
      _originalRush = _task.rush;
    }

    private void ConfigureAccessRights()
    {
      if (App.isDev())
      {
        SprintButtonVisibility = Visibility.Collapsed;
        LogButtonVisibility = Visibility.Collapsed;
        IsTitleReadOnly = true;
        IsManagerEnabled = false;
        IsDeveloperEnabled = false;
        IsSprintEnabled = false;
        IsLeedReadOnly = true;
        IsRushEnabled = false;
        IsNoteReadOnly = true;
      }
      else
      {
        IsTitleReadOnly = false;
        IsManagerEnabled = false;
        IsDeveloperEnabled = true;
        IsSprintEnabled = true;
        IsLeedReadOnly = false;
        IsRushEnabled = true;
        IsNoteReadOnly = false;
      }
    }

    private void Save()
    {
      StringBuilder err = new StringBuilder();

      if (string.IsNullOrWhiteSpace(TaskTitle))
        err.AppendLine("Название не может быть пустой");
      if (string.IsNullOrWhiteSpace(Status))
        err.AppendLine("Укажите статус");
      if (Sprint == null)
        err.AppendLine("Укажите спринт");
      if (Manager == null)
        err.AppendLine("Назначте менеджера");
      if (Developer == null)
        err.AppendLine("Назначте разработчика");

      if (err.Length > 0)
      {
        _dialogService.ShowError(err.ToString(), "Ошибка");
        return;
      }

      _task.title = TaskTitle;
      _task.note = Note;
      _task.status = Status;
      _task.manager = Manager;
      _task.manager_id = Manager?.id;
      _task.developer = Developer;
      _task.developer_id = Developer?.id;
      _task.sprint = Sprint;
      _task.sprint_id = Sprint?.id;
      _task.leed = Leed;
      _task.rush = Rush;

      if (_isNewTask)
        _dataService.AddTask(_task);

      try
      {
        _dataService.SaveChanges();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
        return;
      }

      if (!_isNewTask)
      {
        LogChanges(_task.id);
      }

      _navigationService.GoBack();
    }

    private void GoBack()
    {
      _navigationService.GoBack();
    }

    private void GoToSprints()
    {
      _navigationService.NavigateTo(new pgSprint());
    }

    private void GoToNotes()
    {
      _navigationService.NavigateTo(new pgNote(_task));
    }

    private void GoToLogs()
    {
      _navigationService.NavigateTo(new pgLog(_task));
    }

    private void AssignDeveloper()
    {
      try
      {
        var mostAvailableDeveloper = _dataService.GetMostAvailableDeveloper();
        
        if (mostAvailableDeveloper == null)
        {
          _dialogService.ShowWarning("Не найдено ни одного разработчика в системе.", "Предупреждение");
          return;
        }

        Developer = mostAvailableDeveloper;
        _dialogService.ShowInfo($"Назначен разработчик: {mostAvailableDeveloper.name}", "Исполнитель подобран");
      }
      catch (Exception ex)
      {
        _dialogService.ShowError($"Ошибка при подборе исполнителя: {ex.Message}", "Ошибка");
      }
    }

    private void LogChanges(long taskId)
    {
      var changes = new List<Dictionary<string, string>>();

      if (_originalTitle != _task.title)
        changes.Add(new Dictionary<string, string> { { "field", "Название" }, { "old", _originalTitle ?? "" }, { "new", _task.title ?? "" } });

      if (_originalNote != _task.note)
        changes.Add(new Dictionary<string, string> { { "field", "Текст" }, { "old", _originalNote ?? "" }, { "new", _task.note ?? "" } });

      if (_originalStatus != _task.status)
        changes.Add(new Dictionary<string, string> { { "field", "Статус" }, { "old", _originalStatus ?? "" }, { "new", _task.status ?? "" } });

      if (_originalDeveloperId != _task.developer_id)
      {
        string oldDev = _originalDeveloperId.HasValue ? _dataService.GetUserById(_originalDeveloperId.Value)?.name ?? "" : "";
        string newDev = _task.developer?.name ?? "";
        changes.Add(new Dictionary<string, string> { { "field", "Разработчик" }, { "old", oldDev }, { "new", newDev } });
      }

      if (_originalManagerId != _task.manager_id)
      {
        string oldMan = _originalManagerId.HasValue ? _dataService.GetUserById(_originalManagerId.Value)?.name ?? "" : "";
        string newMan = _task.manager?.name ?? "";
        changes.Add(new Dictionary<string, string> { { "field", "Менеджер" }, { "old", oldMan }, { "new", newMan } });
      }

      if (_originalSprintId != _task.sprint_id)
      {
        string oldSprint = _originalSprintId.HasValue ? _dataService.GetSprintById(_originalSprintId.Value)?.name ?? "" : "";
        string newSprint = _task.sprint?.name ?? "";
        changes.Add(new Dictionary<string, string> { { "field", "Спринт" }, { "old", oldSprint }, { "new", newSprint } });
      }

      if (_originalLeed != _task.leed)
        changes.Add(new Dictionary<string, string> { { "field", "Время выполнения" }, { "old", _originalLeed?.ToString() ?? "" }, { "new", _task.leed?.ToString() ?? "" } });

      if (_originalRush != _task.rush)
        changes.Add(new Dictionary<string, string> { { "field", "Срочная" }, { "old", _originalRush ? "Да" : "Нет" }, { "new", _task.rush ? "Да" : "Нет" } });

      if (App.loginUser == null || App.loginUser.id == 0 || taskId == 0 || changes.Count == 0)
        return;

      try
      {
        var jsonBuilder = new StringBuilder();
        jsonBuilder.Append("[");
        for (int i = 0; i < changes.Count; i++)
        {
          var change = changes[i];
          jsonBuilder.Append("{");
          jsonBuilder.Append($"\"field\":\"{EscapeJson(change["field"])}\",");
          jsonBuilder.Append($"\"old\":\"{EscapeJson(change["old"])}\",");
          jsonBuilder.Append($"\"new\":\"{EscapeJson(change["new"])}\"");
          jsonBuilder.Append("}");
          if (i < changes.Count - 1) jsonBuilder.Append(",");
        }
        jsonBuilder.Append("]");

        _dataService.AddLog(taskId, App.loginUser.id, jsonBuilder.ToString());
      }
      catch (Exception ex)
      {
        _dialogService.ShowWarning($"Ошибка при сохранении логов: {ex.Message}", "Предупреждение");
      }
    }

    private string EscapeJson(string value)
    {
      if (value == null) return "";
      return value.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\r", "\\r")
                  .Replace("\t", "\\t");
    }
  }
}


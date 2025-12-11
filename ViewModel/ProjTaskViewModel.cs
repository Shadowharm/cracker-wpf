using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class ProjTaskViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<Project> _projects;
    private ObservableCollection<Task> _tasks;
    private ObservableCollection<StatusFilter> _statusFilters;
    private Project _selectedProject;
    private Task _selectedTask;
    private string _currentStatus = "";

    private Visibility _projectsButtonVisibility;
    private Visibility _addTaskButtonVisibility;
    private Visibility _reportButtonVisibility;

    public ProjTaskViewModel(IDataService dataService, IDialogService dialogService, INavigationService navigationService)
    {
      _dataService = dataService;
      _dialogService = dialogService;
      _navigationService = navigationService;

      GoToProjectsCommand = new RelayCommand(GoToProjects);
      AddTaskCommand = new RelayCommand(AddTask, () => SelectedProject != null);
      OpenTaskCommand = new RelayCommand<Task>(OpenTask);
      FilterByStatusCommand = new RelayCommand<string>(FilterByStatus);
      OpenReportCommand = new RelayCommand(OpenReport, () => SelectedProject != null);

      if (App.isDev())
      {
        ProjectsButtonVisibility = Visibility.Collapsed;
        AddTaskButtonVisibility = Visibility.Collapsed;
        ReportButtonVisibility = Visibility.Collapsed;
      }
      else
      {
        ProjectsButtonVisibility = Visibility.Visible;
        AddTaskButtonVisibility = Visibility.Visible;
        ReportButtonVisibility = Visibility.Visible;
      }

      StatusFilters = new ObservableCollection<StatusFilter>();
    }

    public ObservableCollection<Project> Projects
    {
      get => _projects;
      set => SetProperty(ref _projects, value);
    }

    public ObservableCollection<Task> Tasks
    {
      get => _tasks;
      set => SetProperty(ref _tasks, value);
    }

    public ObservableCollection<StatusFilter> StatusFilters
    {
      get => _statusFilters;
      set => SetProperty(ref _statusFilters, value);
    }

    public Project SelectedProject
    {
      get => _selectedProject;
      set
      {
        if (SetProperty(ref _selectedProject, value))
        {
          LoadTasks();
        }
      }
    }

    public Task SelectedTask
    {
      get => _selectedTask;
      set => SetProperty(ref _selectedTask, value);
    }

    public Visibility ProjectsButtonVisibility
    {
      get => _projectsButtonVisibility;
      set => SetProperty(ref _projectsButtonVisibility, value);
    }

    public Visibility AddTaskButtonVisibility
    {
      get => _addTaskButtonVisibility;
      set => SetProperty(ref _addTaskButtonVisibility, value);
    }

    public Visibility ReportButtonVisibility
    {
      get => _reportButtonVisibility;
      set => SetProperty(ref _reportButtonVisibility, value);
    }

    public ICommand GoToProjectsCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand OpenTaskCommand { get; }
    public ICommand FilterByStatusCommand { get; }
    public ICommand OpenReportCommand { get; }

    public void LoadData()
    {
      var projects = _dataService.GetProjects();
      Projects = new ObservableCollection<Project>(projects);
    }

    private void LoadTasks()
    {
      if (SelectedProject == null)
      {
        Tasks = new ObservableCollection<Task>();
        StatusFilters.Clear();
        return;
      }

      IEnumerable<Task> tasks;
      Dictionary<string, int> statusCounts;

      if (string.IsNullOrEmpty(_currentStatus))
      {
        if (App.isDev())
        {
          tasks = _dataService.GetTasksByDeveloperAndProject(App.loginUser.id, SelectedProject.id);
          statusCounts = _dataService.GetTaskStatusCountsByDeveloperAndProject(App.loginUser.id, SelectedProject.id);
        }
        else
        {
          tasks = _dataService.GetTasksByProject(SelectedProject.id);
          statusCounts = _dataService.GetTaskStatusCountsByProject(SelectedProject.id);
        }

        StatusFilters.Clear();
        foreach (var status in statusCounts)
        {
          StatusFilters.Add(new StatusFilter
          {
            Status = status.Key,
            DisplayText = $"{status.Key} ({status.Value})"
          });
        }
      }
      else
      {
        if (App.isDev())
        {
          tasks = _dataService.GetTasksByDeveloperAndProject(App.loginUser.id, SelectedProject.id, _currentStatus);
        }
        else
        {
          var allTasks = _dataService.GetTasksByProject(SelectedProject.id);
          tasks = allTasks.Where(t => t.status == _currentStatus);
        }

        StatusFilters.Clear();
        StatusFilters.Add(new StatusFilter
        {
          Status = "",
          DisplayText = "Все"
        });
      }

      Tasks = new ObservableCollection<Task>(tasks);
    }

    private void GoToProjects()
    {
      _navigationService.NavigateTo(new pgProjects());
    }

    private void AddTask()
    {
      if (SelectedProject == null) return;
      _navigationService.NavigateTo(new pgTask(null, SelectedProject));
    }

    private void OpenTask(Task task)
    {
      if (task == null || SelectedProject == null) return;
      _navigationService.NavigateTo(new pgTask(task, SelectedProject));
    }

    private void FilterByStatus(string status)
    {
      _currentStatus = status;
      LoadTasks();
    }

    private void OpenReport()
    {
      if (SelectedProject == null) return;
      _navigationService.NavigateTo(new pgReport(SelectedProject));
    }
  }

  public class StatusFilter
  {
    public string Status { get; set; }
    public string DisplayText { get; set; }
  }
}


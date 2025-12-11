using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class ProjectsViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    
    private ObservableCollection<Project> _projects;
    private Project _selectedProject;

    public ProjectsViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      AddCommand = new RelayCommand(AddProject);
      RemoveCommand = new RelayCommand(RemoveProject, () => SelectedProject != null);
      SaveCommand = new RelayCommand(SaveChanges);
    }

    public ObservableCollection<Project> Projects
    {
      get => _projects;
      set => SetProperty(ref _projects, value);
    }

    public Project SelectedProject
    {
      get => _selectedProject;
      set => SetProperty(ref _selectedProject, value);
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand SaveCommand { get; }

    public void LoadData()
    {
      var projects = _dataService.GetProjects();
      Projects = new ObservableCollection<Project>(projects);
    }

    private void AddProject()
    {
      var project = new Project
      {
        name = "Проект"
      };

      _dataService.AddProject(project);

      try
      {
        _dataService.SaveChanges();
        LoadData();
        SelectedProject = Projects.FirstOrDefault(p => p.id == project.id);
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }

    private void RemoveProject()
    {
      if (SelectedProject == null) return;

      if (!_dialogService.ShowConfirmation("Вы уверены?", "Удалить"))
        return;

      _dataService.DeleteProject(SelectedProject);

      try
      {
        _dataService.SaveChanges();
        LoadData();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }

    public void SaveChanges()
    {
      try
      {
        _dataService.SaveChanges();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }
  }
}


using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class SprintViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;

    private ObservableCollection<Sprint> _sprints;
    private Sprint _selectedSprint;

    public SprintViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      AddCommand = new RelayCommand(AddSprint);
      RemoveCommand = new RelayCommand(RemoveSprint, () => SelectedSprint != null);
    }

    public ObservableCollection<Sprint> Sprints
    {
      get => _sprints;
      set => SetProperty(ref _sprints, value);
    }

    public Sprint SelectedSprint
    {
      get => _selectedSprint;
      set => SetProperty(ref _selectedSprint, value);
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public void LoadData()
    {
      var sprints = _dataService.GetSprints();
      Sprints = new ObservableCollection<Sprint>(sprints);
    }

    private void AddSprint()
    {
      var sprint = new Sprint
      {
        name = "Спринт",
        start_time = DateTime.Now,
        end_time = DateTime.Now.AddDays(7)
      };

      _dataService.AddSprint(sprint);

      try
      {
        _dataService.SaveChanges();
        LoadData();
        SelectedSprint = Sprints.FirstOrDefault(s => s.id == sprint.id);
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message);
      }
    }

    private void RemoveSprint()
    {
      if (SelectedSprint == null) return;

      if (!_dialogService.ShowConfirmation("Вы уверены?", "Удалить"))
        return;

      _dataService.DeleteSprint(SelectedSprint);

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


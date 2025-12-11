using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Cracker.Services;
using Microsoft.Win32;

namespace Cracker.ViewModel
{
  public class GroupingOption
  {
    public string Key { get; set; }
    public string DisplayName { get; set; }

    public override string ToString()
    {
      return DisplayName ?? base.ToString();
    }
  }

  public class ReportViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;

    private Project _project;
    private ObservableCollection<ProjectReportItem> _reportItems;
    private ObservableCollection<UserReportSummary> _summaryItems;
    private ObservableCollection<GroupingOption> _groupingOptions;
    private GroupingOption _selectedGrouping;

    public ReportViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      LoadReportCommand = new RelayCommand(LoadReport);
      ExportCsvCommand = new RelayCommand(ExportToCsv);

      GroupingOptions = new ObservableCollection<GroupingOption>
      {
        new GroupingOption { Key = "date", DisplayName = "По дате создания" },
        new GroupingOption { Key = "developer", DisplayName = "По исполнителю" },
        new GroupingOption { Key = "status", DisplayName = "По статусу" }
      };
      SelectedGrouping = GroupingOptions[0];
    }

    public string PageTitle => $"Отчет по проекту: {_project?.name}";

    public ObservableCollection<ProjectReportItem> ReportItems
    {
      get => _reportItems;
      set => SetProperty(ref _reportItems, value);
    }

    public ObservableCollection<UserReportSummary> SummaryItems
    {
      get => _summaryItems;
      set => SetProperty(ref _summaryItems, value);
    }

    public ObservableCollection<GroupingOption> GroupingOptions
    {
      get => _groupingOptions;
      set => SetProperty(ref _groupingOptions, value);
    }

    public GroupingOption SelectedGrouping
    {
      get => _selectedGrouping;
      set => SetProperty(ref _selectedGrouping, value);
    }

    public ICommand LoadReportCommand { get; }
    public ICommand ExportCsvCommand { get; }

    public void Initialize(Project project)
    {
      _project = project;
      OnPropertyChanged(nameof(PageTitle));
      LoadReport();
    }

    private void LoadReport()
    {
      if (_project == null) return;

      try
      {
        var items = _dataService.GetProjectReport(_project.id);
        ReportItems = new ObservableCollection<ProjectReportItem>(items);

        var summary = _dataService.GetProjectReportSummary(_project.id);
        SummaryItems = new ObservableCollection<UserReportSummary>(summary);
      }
      catch (Exception ex)
      {
        _dialogService.ShowError($"Ошибка загрузки отчета: {ex.Message}");
      }
    }

    private void ExportToCsv()
    {
      if (ReportItems == null || ReportItems.Count == 0)
      {
        _dialogService.ShowMessage("Нет данных для экспорта", "Экспорт");
        return;
      }

      var saveDialog = new SaveFileDialog
      {
        Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
        DefaultExt = "csv",
        FileName = $"Отчет_{_project?.name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
      };

      if (saveDialog.ShowDialog() == true)
      {
        try
        {
          var sb = new StringBuilder();

          sb.AppendLine($"Отчет по проекту: {_project?.name}");
          sb.AppendLine($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}");
          sb.AppendLine($"Группировка: {SelectedGrouping?.DisplayName}");
          sb.AppendLine();

          IEnumerable<IGrouping<string, ProjectReportItem>> groupedData;
          
          switch (SelectedGrouping?.Key)
          {
            case "developer":
              groupedData = ReportItems.GroupBy(r => r.DeveloperName ?? "Не назначен");
              break;
            case "status":
              groupedData = ReportItems.GroupBy(r => r.Status ?? "Без статуса");
              break;
            case "date":
            default:
              groupedData = ReportItems.GroupBy(r => r.CreatedDate.ToString("dd.MM.yyyy"));
              break;
          }

          int totalHours = 0;

          IEnumerable<IGrouping<string, ProjectReportItem>> sortedGroups;
          if (SelectedGrouping?.Key == "date")
          {
            sortedGroups = groupedData.OrderBy(g => 
            {
              var firstItem = g.FirstOrDefault();
              return firstItem?.CreatedDate ?? DateTime.MinValue;
            });
          }
          else
          {
            sortedGroups = groupedData.OrderBy(g => g.Key);
          }

          foreach (var group in sortedGroups)
          {
            sb.AppendLine($"{group.Key}");
            sb.AppendLine("ID;Задача;Статус;Исполнитель;Оценка (ч.);Срочная");

            foreach (var item in group.OrderBy(i => i.CreatedDate))
            {
              var taskTitle = item.TaskTitle?.Replace(";", ",") ?? "";
              var rush = item.IsRush ? "Да" : "Нет";
              sb.AppendLine($"{item.TaskId};{taskTitle};{item.Status};{item.DeveloperName};{item.EstimatedHours};{rush}");

              if (item.EstimatedHours.HasValue)
              {
                totalHours += item.EstimatedHours.Value;
              }
            }

            sb.AppendLine();
          }

          sb.AppendLine($"ИТОГО ЧАСОВ: {totalHours}");

          System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);

          _dialogService.ShowMessage($"Отчет успешно сохранен:\n{saveDialog.FileName}", "Экспорт");
        }
        catch (Exception ex)
        {
          _dialogService.ShowError($"Ошибка экспорта: {ex.Message}");
        }
      }
    }
  }
}

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cracker.Services;

namespace Cracker.ViewModel
{
  public class NoteViewModel : ViewModelBase
  {
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;

    private Task _task;
    private ObservableCollection<Note> _notes;
    private string _newCommentText;

    public NoteViewModel(IDataService dataService, IDialogService dialogService)
    {
      _dataService = dataService;
      _dialogService = dialogService;

      SendCommand = new RelayCommand(SendComment, () => !string.IsNullOrWhiteSpace(NewCommentText));
    }

    public string PageTitle => $"Комментарии к {_task?.title}";

    public ObservableCollection<Note> Notes
    {
      get => _notes;
      set => SetProperty(ref _notes, value);
    }

    public string NewCommentText
    {
      get => _newCommentText;
      set => SetProperty(ref _newCommentText, value);
    }

    public ICommand SendCommand { get; }

    public void Initialize(Task task)
    {
      _task = task;
      LoadData();
    }

    public void LoadData()
    {
      if (_task == null) return;

      var notes = _dataService.GetNotesByTask(_task.id);
      Notes = new ObservableCollection<Note>(notes);
    }

    private void SendComment()
    {
      var commentText = NewCommentText?.Trim();

      if (string.IsNullOrWhiteSpace(commentText))
      {
        _dialogService.ShowWarning("Введите текст комментария", "Предупреждение");
        return;
      }

      var note = new Note
      {
        created = DateTime.Now,
        task_id = _task.id,
        user_id = App.loginUser.id,
        note = commentText
      };

      _dataService.AddNote(note);

      try
      {
        _dataService.SaveChanges();
        NewCommentText = string.Empty;
        LoadData();
      }
      catch (Exception ex)
      {
        _dialogService.ShowError(ex.Message, "Ошибка");
      }
    }
  }
}


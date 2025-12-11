using Cracker.ViewModel;

namespace Cracker.Services
{
  public class ServiceLocator
  {
    private static ServiceLocator _instance;
    private static readonly object _lock = new object();

    private CrackerDB _database;
    private IDataService _dataService;
    private IDialogService _dialogService;
    private NavigationService _navigationService;

    public static ServiceLocator Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            if (_instance == null)
            {
              _instance = new ServiceLocator();
            }
          }
        }
        return _instance;
      }
    }

    private ServiceLocator()
    {
    }

    public void Initialize(CrackerDB database)
    {
      _database = database;
      _dataService = new DataService(_database);
      _dialogService = new DialogService();
      _navigationService = new NavigationService();
    }

    public CrackerDB Database => _database;
    public IDataService DataService => _dataService;
    public IDialogService DialogService => _dialogService;
    public NavigationService NavigationService => _navigationService;

    public MainWindowViewModel CreateMainWindowViewModel()
    {
      return new MainWindowViewModel(_navigationService, _dialogService, _dataService);
    }

    public UsersViewModel CreateUsersViewModel()
    {
      return new UsersViewModel(_dataService, _dialogService);
    }

    public ProjectsViewModel CreateProjectsViewModel()
    {
      return new ProjectsViewModel(_dataService, _dialogService);
    }

    public ProjTaskViewModel CreateProjTaskViewModel()
    {
      return new ProjTaskViewModel(_dataService, _dialogService, _navigationService);
    }

    public TaskViewModel CreateTaskViewModel()
    {
      return new TaskViewModel(_dataService, _dialogService, _navigationService);
    }

    public SprintViewModel CreateSprintViewModel()
    {
      return new SprintViewModel(_dataService, _dialogService);
    }

    public NoteViewModel CreateNoteViewModel()
    {
      return new NoteViewModel(_dataService, _dialogService);
    }

    public LogViewModel CreateLogViewModel()
    {
      return new LogViewModel(_dataService);
    }

    public LoginViewModel CreateLoginViewModel()
    {
      return new LoginViewModel(_dataService, _dialogService);
    }

    public ReportViewModel CreateReportViewModel()
    {
      return new ReportViewModel(_dataService, _dialogService);
    }
  }
}


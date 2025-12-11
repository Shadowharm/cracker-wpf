using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cracker;

namespace Cracker.Services
{
  public class DataService : IDataService
  {
    private readonly CrackerDB _context;

    public DataService(CrackerDB context)
    {
      _context = context;
    }

    #region Users

    public IEnumerable<User> GetUsers()
    {
      return _context.users.OrderBy(u => u.name).ToList();
    }

    public User GetUserById(long id)
    {
      return _context.users.Find(id);
    }

    public User GetUserByCredentials(string login, string password)
    {
      return _context.users.FirstOrDefault(u => u.login == login && u.passwd == password);
    }

    public void AddUser(User user)
    {
      _context.users.Add(user);
    }

    public void UpdateUser(User user)
    {
      _context.Entry(user).State = EntityState.Modified;
    }

    public void DeleteUser(User user)
    {
      _context.users.Remove(user);
    }

    public int GetUsersCount()
    {
      return _context.users.Count();
    }

    #endregion

    #region Projects

    public IEnumerable<Project> GetProjects()
    {
      _context.projects.Load();
      return _context.projects.OrderBy(p => p.name).ToList();
    }

    public Project GetProjectById(long id)
    {
      return _context.projects.Find(id);
    }

    public void AddProject(Project project)
    {
      _context.projects.Add(project);
    }

    public void UpdateProject(Project project)
    {
      _context.Entry(project).State = EntityState.Modified;
    }

    public void DeleteProject(Project project)
    {
      _context.projects.Remove(project);
    }

    #endregion

    #region Tasks

    public IEnumerable<Task> GetTasks()
    {
      _context.tasks.Load();
      return _context.tasks.ToList();
    }

    public IEnumerable<Task> GetTasksByProject(long projectId)
    {
      return _context.tasks.Where(t => t.project_id == projectId).OrderByDescending(t => t.created).ToList();
    }

    public IEnumerable<Task> GetTasksByDeveloper(long developerId)
    {
      return _context.tasks.Where(t => t.developer_id == developerId).OrderByDescending(t => t.created).ToList();
    }

    public IEnumerable<Task> GetTasksByManager(long managerId)
    {
      return _context.tasks.Where(t => t.manager_id == managerId).OrderByDescending(t => t.created).ToList();
    }

    public IEnumerable<Task> GetTasksByDeveloperAndProject(long developerId, long projectId, string status = null)
    {
      var query = _context.tasks.Where(t => t.developer_id == developerId && t.project_id == projectId);
      if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.status == status);
      return query.OrderByDescending(t => t.created).ToList();
    }

    public IEnumerable<Task> GetTasksByManagerAndProject(long managerId, long projectId, string status = null)
    {
      var query = _context.tasks.Where(t => t.manager_id == managerId && t.project_id == projectId);
      if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.status == status);
      return query.OrderByDescending(t => t.created).ToList();
    }

    public Dictionary<string, int> GetTaskStatusCountsByDeveloperAndProject(long developerId, long projectId)
    {
      return _context.tasks
        .Where(t => t.developer_id == developerId && t.project_id == projectId)
        .GroupBy(t => t.status)
        .OrderBy(g => g.Key)
        .ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<string, int> GetTaskStatusCountsByManagerAndProject(long managerId, long projectId)
    {
      return _context.tasks
        .Where(t => t.manager_id == managerId && t.project_id == projectId)
        .GroupBy(t => t.status)
        .OrderBy(g => g.Key)
        .ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<string, int> GetTaskStatusCountsByProject(long projectId)
    {
      return _context.tasks
        .Where(t => t.project_id == projectId)
        .GroupBy(t => t.status)
        .OrderBy(g => g.Key)
        .ToDictionary(g => g.Key, g => g.Count());
    }

    public Task GetTaskById(long id)
    {
      return _context.tasks.Find(id);
    }

    public void AddTask(Task task)
    {
      _context.tasks.Add(task);
    }

    public void UpdateTask(Task task)
    {
      _context.Entry(task).State = EntityState.Modified;
    }

    public void DeleteTask(Task task)
    {
      _context.tasks.Remove(task);
    }

    #endregion

    #region Sprints

    public IEnumerable<Sprint> GetSprints()
    {
      return _context.sprints.OrderBy(s => s.name).ToList();
    }

    public Sprint GetSprintById(long id)
    {
      return _context.sprints.Find(id);
    }

    public void AddSprint(Sprint sprint)
    {
      _context.sprints.Add(sprint);
    }

    public void UpdateSprint(Sprint sprint)
    {
      _context.Entry(sprint).State = EntityState.Modified;
    }

    public void DeleteSprint(Sprint sprint)
    {
      _context.sprints.Remove(sprint);
    }

    #endregion

    #region Notes

    public IEnumerable<Note> GetNotesByTask(long taskId)
    {
      _context.users.Load();
      return _context.notes.Where(n => n.task_id == taskId).OrderBy(n => n.created).ToList();
    }

    public void AddNote(Note note)
    {
      _context.notes.Add(note);
    }

    #endregion

    #region Logs

    public IEnumerable<Log> GetLogsByTask(long taskId)
    {
      _context.users.Load();
      return _context.logs.Where(l => l.task_id == taskId).OrderByDescending(l => l.time).ToList();
    }

    public void AddLog(long taskId, long userId, string jsonValue)
    {
      string sql = "INSERT INTO logs (task_id, user_id, time, value) VALUES (@p0, @p1, @p2, (@p3::text)::json)";
      _context.Database.ExecuteSqlCommand(sql, taskId, userId, DateTime.Now, jsonValue);
    }

    #endregion

    #region Users by role

    public IEnumerable<User> GetManagers()
    {
      return _context.users.Where(u => u.role == "Менеджер").OrderBy(u => u.name).ToList();
    }

    public IEnumerable<User> GetDevelopers()
    {
      return _context.users.Where(u => u.role == "Разработчик").OrderBy(u => u.name).ToList();
    }

    public User GetMostAvailableDeveloper()
    {
      var developers = _context.users
        .Where(u => u.role == "Разработчик")
        .ToList();
      
      if (!developers.Any())
        return null;

      var activeStatuses = new[] { "Новая", "Готова к работе", "В работе", "На обсуждении", 
                                   "Ревью", "Выливка", "Проверка", "Проверена", "Релиз", 
                                   "Возвращена", "Доработка" };

      var developerWorkloads = developers.Select(dev => new
      {
        Developer = dev,
        ActiveTaskCount = _context.tasks.Count(t => 
          t.developer_id == dev.id && 
          activeStatuses.Contains(t.status))
      }).OrderBy(x => x.ActiveTaskCount)
        .ThenBy(x => x.Developer.name)
        .ToList();

      var selectedDeveloper = developerWorkloads.FirstOrDefault()?.Developer;
            if (selectedDeveloper != null && selectedDeveloper.role != "Разработчик")
        return null;
      
      return selectedDeveloper;
    }

    #endregion

    #region Reports

    public IEnumerable<ProjectReportItem> GetProjectReport(long projectId)
    {
      _context.users.Load();

      var tasks = _context.tasks
        .Where(t => t.project_id == projectId)
        .OrderByDescending(t => t.created)
        .ToList();

      return tasks.Select(t => new ProjectReportItem
      {
        TaskId = t.id,
        TaskTitle = t.title,
        TaskNote = t.note,
        Status = t.status,
        DeveloperName = t.developer?.name ?? "Не назначен",
        ManagerName = t.manager?.name ?? "Не назначен",
        CreatedDate = t.created,
        EstimatedHours = t.leed,
        IsRush = t.rush
      }).ToList();
    }

    public IEnumerable<UserReportSummary> GetProjectReportSummary(long projectId)
    {
      _context.users.Load();

      var tasks = _context.tasks
        .Where(t => t.project_id == projectId)
        .ToList();

      var completedStatuses = new[] { "Готова", "Завершена", "Закрыта" };

      var groupedByDeveloper = tasks
        .Where(t => t.developer_id.HasValue)
        .GroupBy(t => t.developer)
        .Select(g => new UserReportSummary
        {
          UserName = g.Key?.name ?? "Не назначен",
          TaskCount = g.Count(),
          CompletedTaskCount = g.Count(t => completedStatuses.Contains(t.status)),
          TotalEstimatedHours = g.Where(t => t.leed.HasValue).Sum(t => t.leed.Value)
        })
        .OrderByDescending(s => s.TaskCount)
        .ToList();

      return groupedByDeveloper;
    }

    #endregion

    public void SaveChanges()
    {
      _context.SaveChanges();
    }
  }
}

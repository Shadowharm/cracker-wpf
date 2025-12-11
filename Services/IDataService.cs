using System;
using System.Collections.Generic;
using Cracker;

namespace Cracker.Services
{
  public interface IDataService
  {
    IEnumerable<User> GetUsers();
    User GetUserById(long id);
    User GetUserByCredentials(string login, string password);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
    int GetUsersCount();

    IEnumerable<Project> GetProjects();
    Project GetProjectById(long id);
    void AddProject(Project project);
    void UpdateProject(Project project);
    void DeleteProject(Project project);

    IEnumerable<Task> GetTasks();
    IEnumerable<Task> GetTasksByProject(long projectId);
    IEnumerable<Task> GetTasksByDeveloper(long developerId);
    IEnumerable<Task> GetTasksByManager(long managerId);
    IEnumerable<Task> GetTasksByDeveloperAndProject(long developerId, long projectId, string status = null);
    IEnumerable<Task> GetTasksByManagerAndProject(long managerId, long projectId, string status = null);
    Dictionary<string, int> GetTaskStatusCountsByDeveloperAndProject(long developerId, long projectId);
    Dictionary<string, int> GetTaskStatusCountsByManagerAndProject(long managerId, long projectId);
    Dictionary<string, int> GetTaskStatusCountsByProject(long projectId);
    Task GetTaskById(long id);
    void AddTask(Task task);
    void UpdateTask(Task task);
    void DeleteTask(Task task);

    IEnumerable<Sprint> GetSprints();
    Sprint GetSprintById(long id);
    void AddSprint(Sprint sprint);
    void UpdateSprint(Sprint sprint);
    void DeleteSprint(Sprint sprint);

    IEnumerable<Note> GetNotesByTask(long taskId);
    void AddNote(Note note);

    IEnumerable<Log> GetLogsByTask(long taskId);
    void AddLog(long taskId, long userId, string jsonValue);

    IEnumerable<User> GetManagers();
    IEnumerable<User> GetDevelopers();
    User GetMostAvailableDeveloper();

    IEnumerable<ProjectReportItem> GetProjectReport(long projectId);
    IEnumerable<UserReportSummary> GetProjectReportSummary(long projectId);

    void SaveChanges();
  }
}


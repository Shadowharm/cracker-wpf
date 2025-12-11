using System;

namespace Cracker
{
  public class ProjectReportItem
  {
    public long TaskId { get; set; }

    public string TaskTitle { get; set; }

    public string TaskNote { get; set; }

    public string Status { get; set; }

    public string DeveloperName { get; set; }

    public string ManagerName { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? EstimatedHours { get; set; }

    public bool IsRush { get; set; }
  }

  public class UserReportSummary
  {
    public string UserName { get; set; }

    public int TaskCount { get; set; }

    public int CompletedTaskCount { get; set; }

    public int TotalEstimatedHours { get; set; }
  }
}

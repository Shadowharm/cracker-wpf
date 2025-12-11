using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("tasks")]
  public class Task
  {
    [Key]
    public long id { get; set; }
    public long? developer_id { get; set; }
    public long? manager_id { get; set; }
    public long? project_id { get; set; }
    public long? sprint_id { get; set; }
    public string title { get; set; }
    public string note { get; set; }
    public string status { get; set; }
    public DateTime created { get; set; }
    public bool rush { get; set; }
    public int? leed { get; set; }

    [ForeignKey("developer_id")]
    public User developer { get; set; }
    [ForeignKey("manager_id")]
    public User manager { get; set; }
    [ForeignKey("project_id")]
    public Project project { get; set; }
    [ForeignKey("sprint_id")]
    public Sprint sprint { get; set; }

    public virtual ICollection<Note> notes { get; set; }
    public virtual ICollection<Log> logs { get; set; }
    public virtual ICollection<File> files { get; set; }

    public override string ToString() => $"{title}";
  }
}
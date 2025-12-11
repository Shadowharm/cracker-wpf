using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("notes")]
  public class Note
  {
    [Key]
    public long id { get; set; }
    public long? task_id { get; set; }
    public long? user_id { get; set; }
    public string note { get; set; }
    public DateTime created { get; set; }

    [ForeignKey("user_id")]
    public User user { get; set; }
    [ForeignKey("task_id")]
    public Task task { get; set; }

    public override string ToString() => $"{created.ToString()} {user}";
  }
}
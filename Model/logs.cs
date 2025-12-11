using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("logs")]
  public class Log
  {
    [Key]
    public long id { get; set; }
    public long? task_id { get; set; }
    public long? user_id { get; set; }

    public DateTime time { get; set; }
    public string value { get; set; }

    [ForeignKey("user_id")]
    public User user { get; set; }
    [ForeignKey("task_id")]
    public Task task { get; set; }

    public override string ToString() => $"{time.ToString()}";
  }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("sprints")]
  public class Sprint
  {
    [Key]
    public long id { get; set; }
    public string name { get; set; }
    public DateTime start_time { get; set; }
    public DateTime end_time { get; set; }

    public virtual ICollection<Task> tasks { get; set; }

    public override string ToString() => $"{name} ({start_time.ToString("dd.MM")} - {end_time.ToString("dd.MM")})";
  }
}
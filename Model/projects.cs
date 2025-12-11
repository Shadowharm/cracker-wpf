using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("projects")]
  public class Project
  {
    [Key]
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }

    public virtual ICollection<Task> tasks { get; set; }

    public override string ToString() => $"{name}";
  }
}
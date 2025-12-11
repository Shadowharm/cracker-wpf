using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("users")]
  public class User
  {
    [Key]
    public long id { get; set; }
    public string name { get; set; }
    public string login { get; set; }
    public string role { get; set; }
    public string passwd { get; set; }

    [InverseProperty("developer")]
    public virtual ICollection<Task> tasksD { get; set; } 
    [InverseProperty("manager")]
    public virtual ICollection<Task> tasksM { get; set; } 
    public virtual ICollection<Note> notes { get; set; }
    public virtual ICollection<Log> logs { get; set; }

    public override string ToString() => $"{name}";
  }
}
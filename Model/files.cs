using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cracker
{
  [Table("files")]
  public class File
  {
    [Key]
    public long id { get; set; }
    public long? task_id { get; set; }
    public string name { get; set; }
    public int size { get; set; }
    public string link { get; set; }

    [ForeignKey("iTask")]
    public Task task { get; set; }

    public override string ToString() => $"{link}+\\+{name}";
  }
}


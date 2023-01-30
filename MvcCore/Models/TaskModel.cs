using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCore.Models
{
    [Table("Tasks")]
    public class TaskModel
    {
        [Key]
        public int TaskId { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "Task name is obligatory.")]
        [MaxLength(50)]
        public string Name { get; set; }
        [DisplayName("Description")]
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool Done { get; set; }

    }
}

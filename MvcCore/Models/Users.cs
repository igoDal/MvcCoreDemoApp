using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCore.Models
{
    [Table("Users", Schema = "ManagementStudio")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is obligatory.")]
        [MaxLength(50)]
        public string Name { get; set; }
        [DisplayName("Email")]
        [MaxLength(50)]
        public string Email { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}

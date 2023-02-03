using Microsoft.EntityFrameworkCore;

namespace MvcCore.Models
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options)
        {
        }

        public DbSet<TaskModel> Tasks { get; set; }
    }
}

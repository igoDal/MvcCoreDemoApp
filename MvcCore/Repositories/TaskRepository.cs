using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcCore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCore.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerContext _context;


        public TaskRepository(TaskManagerContext context)
        {
            _context = context;

        }

        public TaskModel Get(int taskId)
            => _context.Tasks.SingleOrDefault(t => t.TaskId == taskId);

        public IQueryable<TaskModel> GetAllActive()
            => _context.Tasks.Where(x => !x.Done);
        public void Add(TaskModel task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void Update(int taskId, TaskModel task)
        {
            var result = _context.Tasks.SingleOrDefault(x => x.TaskId == taskId);
            if (result != null)
            {
                result.Name = task.Name;
                result.Description = task.Description;
                result.Done = task.Done;
                _context.SaveChanges();
            }
        }
        public void Delete(int taskId)
        {
            var result = _context.Tasks.SingleOrDefault(x => x.TaskId == taskId);
            if (result != null)
            {
                _context.Remove(result);
                _context.SaveChanges();
            }
        }

        //public async Task AddUserToRoleAsync(string userId, string roleName)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    var role = await _roleManager.FindByNameAsync(roleName);

        //    var tryAddToRole = await _userManager.AddToRoleAsync(user, roleName);
        //    if (!tryAddToRole.Succeeded)
        //    {
        //        throw new System.Exception("Couldn't add user to role");
        //    }

        //    var userRole = new IdentityUserRole<string>
        //    {
        //        UserId = user.Id,
        //        RoleId = role.Id
        //    };

        //    _context.AspNetUserRole.Add(userRole);
        //    await _context.SaveChangesAsync();
        //}
    }
}

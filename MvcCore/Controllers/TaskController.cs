using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCore.Models;
using MvcCore.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MvcCore.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        // GET: Task
        public ActionResult Index()
        {
            return View(_taskRepository.GetAllActive());
        }

        [Authorize]
        // GET: Task/Details/5
        public ActionResult Details(int id)
        {
            return View(_taskRepository.Get(id));
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            return View(new TaskModel());
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModel taskModel)
        {
            _taskRepository.Add(taskModel);

            return RedirectToAction(nameof(Index));
            
        }

        // GET: Task/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_taskRepository.Get(id));
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TaskModel taskModel)
        {
            _taskRepository.Update(id, taskModel);
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Task/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_taskRepository.Get(id));
        }

        // POST: Task/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TaskModel taskModel)
        {
            _taskRepository.Delete(id);
            return RedirectToAction(nameof(Index));
            
        }

        //GET: Task/Done/5
        public ActionResult Done(int id)
        {
            TaskModel task = _taskRepository.Get(id);
            task.Done = true;
            _taskRepository.Update(id, task);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Authenticate()
        {
            var appClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Igor"),
                new Claim(ClaimTypes.Email, "igor@igor.pl"),
                new Claim("App.Says", "Welcome, Igor. You're verified"),
            };
            
            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Igor D."),
                new Claim("Security", "Welcome, Igor D."),
            };

            var appIdentity = new ClaimsIdentity(appClaims, "App Identity");
            var licenseIdentity = new ClaimsIdentity(appClaims, "Security");

            var userPrincipal = new ClaimsPrincipal(new[] { appIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);


            return RedirectToAction("Index");
        }
    }
}

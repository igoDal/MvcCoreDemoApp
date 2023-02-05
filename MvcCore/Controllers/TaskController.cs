﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcCore.Models;
using MvcCore.Repositories;
using NETCore.MailKit.Core;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCore.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //public TaskController(ITaskRepository taskRepository)
        //{
        //    _taskRepository = taskRepository;
        //}

        //constructor for inMemoryDb
        public TaskController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            ITaskRepository taskRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _taskRepository = taskRepository;
            _emailService = emailService;
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
        [Authorize(Policy ="Claim.DoB")]
        // GET: Task/Details/5
        public ActionResult DetailsPolicy(int id)
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

        //public IActionResult Authenticate()
        //{
        //    var appClaims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Name, "Igor"),
        //        new Claim(ClaimTypes.Email, "igor@igor.pl"),
        //        new Claim("App.Says", "Welcome, Igor. You're verified"),
        //    };
            
        //    var licenseClaims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Name, "Igor D."),
        //        new Claim("Security", "Welcome, Igor D."),
        //    };

        //    var appIdentity = new ClaimsIdentity(appClaims, "App Identity");
        //    var licenseIdentity = new ClaimsIdentity(licenseClaims, "Security");

        //    var userPrincipal = new ClaimsPrincipal(new[] { appIdentity, licenseIdentity });

        //    HttpContext.SignInAsync(userPrincipal);


        //    return RedirectToAction("Index");
        //}

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task <IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction("Index");
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(nameof(VerifyEmail), "Task", new {userId = user.Id, code}, Request.Scheme, Request.Host.ToString());
                await _emailService.SendAsync("test@test.com", "email verify", $"<a href = \"{link}\">Verify Email</a>", true);

                return RedirectToAction("EmailVerification");
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();

            }
            return BadRequest();
        }
        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}

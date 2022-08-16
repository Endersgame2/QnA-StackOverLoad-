using Microsoft.AspNetCore.Mvc;
using QnA.Models.Account;
using QnA.Models.Data;
using QnA.Models.Data.Shared;
using QnA.Services;

namespace QnA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QnADbContext _qnADbContext;
        public AccountController(ILogger<HomeController> logger, QnADbContext qnADbContext)
        {
            _logger = logger;
            _qnADbContext = qnADbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public IActionResult Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Notification.Message = "Validation failed";
                model.Notification.MessageClass = "alert-danger";
                return View(model);
            }
            if(model.Password != model.ConfirmPassword)
            {
                model.Notification.Message = "Passwords must match";
                model.Notification.MessageClass = "alert-danger";
                return View(model);
            }
            var userExists = _qnADbContext.Users.Any(user => user.Email == model.Email || user.Name == model.UserName);
            if (userExists)
            {
                model.Notification.Message = "This email/username is already registered. Please reset your password.";
                model.Notification.MessageClass = "alert-danger";
                return View(model);
            }
            var hashedPassword = HashService.Hash(model.Password);
            var user = new User()
            {
                Email = model.Email,
                PasswordHash = hashedPassword,
                Name = model.UserName
            };
            try
            {
                _qnADbContext.Database.BeginTransaction();
                user = _qnADbContext.Users.Add(user);
                var userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = _qnADbContext.Roles.FirstOrDefault(role => role.Name == "User").Id
                };
                _qnADbContext.UserRoles.Add(userRole);
                _qnADbContext.SaveChanges();
                _qnADbContext.Database.CurrentTransaction.Commit();
            }
            catch(Exception e)
            {
                _logger.LogError($"Register user failed for ${model.UserName}", e);
                _qnADbContext.Database.CurrentTransaction.Rollback();
                return RedirectToAction("Error", "Home");
            }
            model.Notification.Message = "Registered successfully";
            model.Notification.MessageClass = "alert-success";
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Notification.Message = "All fields are required";
                model.Notification.MessageClass = "alert-danger";
                return View(model);
            }
            var users = _qnADbContext.Users.Where(user => user.Email == model.Key || user.Name == model.Key).ToList(); // in theory one user's email can be another user's username, so we do this
            foreach(var user in users)
            {
                if(HashService.Verify(model.Password, user.PasswordHash))
                {
                    var roles = from userRole in _qnADbContext.UserRoles
                                join role in _qnADbContext.Roles on userRole.RoleId equals role.Id
                                where userRole.UserId == user.Id
                                select role.Name;
                    if(!roles.Any())
                    {
                        model.Notification.Message = "Logged in but no roles";
                        model.Notification.MessageClass = "alert-warning";
                        return View(model);
                    }
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetString("Roles", string.Join(",", roles));
                    return RedirectToAction("Index", "Home");
                }
            }
            model.Notification.Message = "Invalid credentials";
            model.Notification.MessageClass = "alert-danger";
            return View(model);
        }
        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Roles");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

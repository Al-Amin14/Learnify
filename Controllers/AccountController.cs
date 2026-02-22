using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Learnify.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Turbo_Food_Main.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            // 🔒 Only allow Student & Teacher for public registration
            var model = new RegisterModel
            {
                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Student", Value = "Student" },
                    new SelectListItem { Text = "Teacher", Value = "Teacher" }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            // Always repopulate roles
            model.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Student", Value = "Student" },
                new SelectListItem { Text = "Teacher", Value = "Teacher" }
            };

            if (!ModelState.IsValid)
                return View(model);

            // 🔐 HARD ROLE VALIDATION
            if (model.Role != "Student" && model.Role != "Teacher")
            {
                ModelState.AddModelError("", "Invalid role selection.");
                return View(model);
            }

            var user = new Users
            {
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email,
                RoleType = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // ✅ Role MUST already exist (seeded in Program.cs)
                await _userManager.AddToRoleAsync(user, model.Role);

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Profile");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // ================= LOGOUT =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ================= PROFILE =================
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            return View(new ProfileModel
            {
                Name = user.FullName,
                Email = user.Email
            });
        }

        // ================= VERIFY EMAIL =================
        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            return RedirectToAction("ChangePassword", new { username = user.UserName });
        }

        // ================= CHANGE PASSWORD =================
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("VerifyEmail");

            return View(new ChangePasswordModel
            {
                Email = username
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.NewPassword);

            return RedirectToAction("Login");
        }
    }
}

using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Turbo_Food_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.Role != "Student" && model.Role != "Teacher")
                return BadRequest("Invalid role selection.");

            var user = new Users
            {
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email,
                RoleType = model.Role
            };
            //hash password
            var passwordHasher = new PasswordHasher<Users>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);


            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // Add role
            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            await _userManager.AddToRoleAsync(user, model.Role);
            await _signInManager.SignInAsync(user, false);

            return Ok(new { message = "User registered successfully", user = user.Email, role = user.RoleType,passwordHashed=user.PasswordHash });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
                return Unauthorized("Invalid login attempt.");

            var user = await _userManager.FindByEmailAsync(model.Email);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.RoleType)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Login successful",
                token = jwt
            });
        }

        // ================= LOGOUT =================
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        // ================= PROFILE =================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            // Get email from JWT token
            var email = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                name = user.FullName,
                email = user.Email,
                role = user.RoleType
            });
        }

        // ================= VERIFY EMAIL =================
        [Authorize]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new { message = "Email exists", username = user.UserName });
        }

        // ================= CHANGE PASSWORD =================
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
                return NotFound("User not found.");

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.NewPassword);

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
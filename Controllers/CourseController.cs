using learnify.Models;
using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        private readonly AppDbContenxt _context;
        private readonly UserManager<Users> _userManager; // Inject UserManager

        public CourseController(AppDbContenxt context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager; // assign injected UserManager
        }

        // POST: api/course
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course model)
        {
            try
            {
                // 1. Get logged-in user's Id from JWT
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                // 2. Verify user exists and is a Teacher
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RoleType != "Teacher")
                    return Forbid("Only teachers can create courses.");

                // 3. Assign Teacher_Id from JWT (AspNetUsers.Id)

                model.Teacher_Id = user.Id;

                // 4. Validate model AFTER setting Teacher_Id
                if (!TryValidateModel(model))
                    return BadRequest(ModelState);

                // 5. Save course
                await _context.Courses.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Course created successfully",
                    courseId = model.Course_Id,
                    teacherId = model.Teacher_Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
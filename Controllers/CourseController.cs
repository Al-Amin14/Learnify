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

        [Authorize]
        [HttpPut("update-description/{id}")]
        public async Task<IActionResult> UpdateCourseDescription(int id, [FromBody] string description)
        {
            try
            {
                // Get logged-in user id from JWT
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                // Find the course
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound("Course not found.");

                // Check if the logged-in teacher owns this course
                if (course.Teacher_Id != userId)
                    return Forbid("You can only update your own course.");

                // Update description
                course.Description = description;

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Course description updated successfully",
                    courseId = course.Course_Id,
                    newDescription = course.Description
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                // Get logged-in user ID from JWT
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                // Find the course
                var course = await _context.Courses.FindAsync(id);

                if (course == null)
                    return NotFound("Course not found.");

                // Ensure the logged-in teacher owns the course
                if (course.Teacher_Id != userId)
                    return Forbid("You can only delete your own courses.");

                // Delete course
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Course deleted successfully",
                    courseId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _context.Courses
                    .Include(c => c.Classes) // optional if you want class info
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
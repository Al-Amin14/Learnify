using learnify.Models;
using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContenxt _context;
        private readonly UserManager<Users> _userManager;

        public CourseController(AppDbContenxt context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // -------- Helper to verify Teacher --------
        private async Task<(Users? user, IActionResult? error)> GetVerifiedTeacher()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return (null, Unauthorized("User ID not found in token."));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RoleType != "Teacher")
                return (null, StatusCode(403, "Only teachers can perform this action."));

            return (user, null);
        }

        // POST: api/course
        [Authorize( Roles ="Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course model)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                model.Teacher_Id = user!.Id;

                if (!TryValidateModel(model))
                    return BadRequest(ModelState);

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
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/course/update-description/{id}
        [Authorize]
        [HttpPut("update-description/{id}")]
        public async Task<IActionResult> UpdateCourseDescription(int id, [FromBody] string description)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound("Course not found.");

                // Ensure teacher owns this course
                if (course.Teacher_Id != user!.Id)
                    return StatusCode(403, "You can only update your own course.");

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
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/course/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound("Course not found.");

                // Ensure teacher owns this course
                if (course.Teacher_Id != user!.Id)
                    return StatusCode(403, "You can only delete your own courses.");

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
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/course
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _context.Courses
                    .Include(c => c.Classes)
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/course/my-courses
        [Authorize]
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var courses = await _context.Courses
                    .Where(c => c.Teacher_Id == user!.Id)
                    .Include(c => c.Classes)
                    .ToListAsync();

                if (!courses.Any())
                    return NotFound("No courses found for this teacher.");

                return Ok(new
                {
                    teacherId = user!.Id,
                    totalCourses = courses.Count,
                    courses = courses.Select(c => new
                    {
                        courseId = c.Course_Id,
                        title = c.Title,
                        description = c.Description,
                        classesId = c.Classes_id,
                        className = c.Classes.ClassesName
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
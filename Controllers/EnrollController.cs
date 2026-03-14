using learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Models;
using System.Security.Claims;

namespace learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollController : Controller
    {
        private readonly AppDbContenxt _context;

        public EnrollController(AppDbContenxt context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] Enrollment model)
        {
            try
            {
                // 1️⃣ Get logged-in student ID from JWT
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                model.Student_Id = studentId;

                // 2️⃣ Validate Course exists
                var course = await _context.Courses.FindAsync(model.Course_Id);
                if (course == null)
                    return NotFound("Course not found.");

                // Optional: Check if student already enrolled
                var alreadyEnrolled = await _context.Enroll
                    .AnyAsync(e => e.Course_Id == model.Course_Id && e.Student_Id == studentId);
                if (alreadyEnrolled)
                    return BadRequest("You are already enrolled in this course.");

                // 3️⃣ Set enrollment date if not provided
                if (model.Enrolled_On == default)
                    model.Enrolled_On = DateTime.Now;

                // 4️⃣ Save enrollment
                await _context.Enroll.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Enrollment successful",
                    enrollmentId = model.Enrollment_Id,
                    studentId = model.Student_Id,
                    courseId = model.Course_Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] Enrollment model)
        {
            try
            {
                // Get student ID from JWT
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var enrollment = await _context.Enroll.FindAsync(id);
                if (enrollment == null)
                    return NotFound("Enrollment not found.");

                // Check ownership
                if (enrollment.Student_Id != studentId)
                    return Forbid("You can only update your own enrollments.");

                // Validate new Course_Id
                var course = await _context.Courses.FindAsync(model.Course_Id);
                if (course == null)
                    return NotFound("Course not found.");

                // Optional: prevent duplicate enrollment
                var exists = await _context.Enroll
                    .AnyAsync(e => e.Course_Id == model.Course_Id && e.Student_Id == studentId && e.Enrollment_Id != id);
                if (exists)
                    return BadRequest("You are already enrolled in this course.");

                // Update Course_Id and date if provided
                enrollment.Course_Id = model.Course_Id;
                enrollment.Enrolled_On = model.Enrolled_On != default ? model.Enrolled_On : enrollment.Enrolled_On;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Enrollment updated successfully",
                    enrollmentId = enrollment.Enrollment_Id,
                    studentId = enrollment.Student_Id,
                    courseId = enrollment.Course_Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var enrollment = await _context.Enroll.FindAsync(id);
                if (enrollment == null)
                    return NotFound("Enrollment not found.");

                // Ensure ownership
                if (enrollment.Student_Id != studentId)
                    return Forbid("You can only delete your own enrollments.");

                _context.Enroll.Remove(enrollment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Enrollment deleted successfully",
                    enrollmentId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var enrollments = await _context.Enroll
                    .Where(e => e.Student_Id == studentId)
                    .Include(e => e.Course) // optional: include course details
                    .ToListAsync();

                return Ok(enrollments.Select(e => new
                {
                    enrollmentId = e.Enrollment_Id,
                    courseId = e.Course_Id,
                    courseTitle = e.Course.Title,
                    enrolledOn = e.Enrolled_On
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

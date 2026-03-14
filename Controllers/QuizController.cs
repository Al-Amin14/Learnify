using learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : Controller
    {

        private readonly AppDbContenxt _context;

        public QuizController(AppDbContenxt context) { 
        
            _context = context;
        }
            
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] Quiz model)
        {
            try
            {
                // Get teacher ID from JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                // Assign teacher id from JWT
                model.TeacherId = userId;

                // Validate model after setting TeacherID
                if (!TryValidateModel(model))
                    return BadRequest(ModelState);

                // Check if course exists
                var course = await _context.Courses.FindAsync(model.Course_Id);
                if (course == null)
                    return NotFound("Course not found.");

                // Optional: ensure teacher owns the course
                if (course.Teacher_Id != userId)
                    return Forbid("You can only create quizzes for your own courses.");

                // Save quiz
                await _context.Quizs.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quiz created successfully",
                    quizId = model.Quiz_Id,
                    teacherId = model.TeacherId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] Quiz model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                var quiz = await _context.Quizs.FindAsync(id);

                if (quiz == null)
                    return NotFound("Quiz not found.");

                // Check ownership of quiz
                if (quiz.TeacherId != userId)
                    return Forbid("You can only update your own quizzes.");

                // Validate Course_Id
                var course = await _context.Courses.FindAsync(model.Course_Id);

                if (course == null)
                    return NotFound("Course not found.");

                // Optional (recommended): ensure teacher owns the course
                if (course.Teacher_Id != userId)
                    return Forbid("You can only assign quizzes to your own courses.");

                // Update fields
                quiz.Title = model.Title;
                quiz.Total_Marks = model.Total_Marks;
                quiz.Question = model.Question;
                quiz.Course_Id = model.Course_Id;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quiz updated successfully",
                    quizId = quiz.Quiz_Id
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                var quiz = await _context.Quizs.FindAsync(id);

                if (quiz == null)
                    return NotFound("Quiz not found.");

                // Check ownership
                if (quiz.TeacherId != userId)
                    return Forbid("You can only delete your own quizzes.");

                _context.Quizs.Remove(quiz);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quiz deleted successfully",
                    quizId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

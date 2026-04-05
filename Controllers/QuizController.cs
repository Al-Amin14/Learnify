using learnify.Models;
using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDbContenxt _context;
        private readonly UserManager<Users> _userManager;

        public QuizController(AppDbContenxt context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // -------- Helper to verify Teacher --------
        private async Task<(Users? user, IActionResult? error)> GetVerifiedTeacher()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return (null, Unauthorized("User ID not found in token."));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RoleType != "Teacher")
                return (null, StatusCode(403, "Only teachers can perform this action."));

            return (user, null);
        }

        // POST: api/quiz
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] Quiz model)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                model.TeacherId = user!.Id;

                if (!TryValidateModel(model))
                    return BadRequest(ModelState);

                var course = await _context.Courses.FindAsync(model.Course_Id);
                if (course == null)
                    return NotFound("Course not found.");

                if (course.Teacher_Id != user.Id)
                    return StatusCode(403, "You can only create quizzes for your own courses.");

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
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/quiz/update/{id}
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] Quiz model)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var quiz = await _context.Quizs.FindAsync(id);
                if (quiz == null)
                    return NotFound("Quiz not found.");

                if (quiz.TeacherId != user!.Id)
                    return StatusCode(403, "You can only update your own quizzes.");

                var course = await _context.Courses.FindAsync(model.Course_Id);
                if (course == null)
                    return NotFound("Course not found.");

                if (course.Teacher_Id != user.Id)
                    return StatusCode(403, "You can only assign quizzes to your own courses.");

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

        // DELETE: api/quiz/delete/{id}
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var quiz = await _context.Quizs.FindAsync(id);
                if (quiz == null)
                    return NotFound("Quiz not found.");

                if (quiz.TeacherId != user!.Id)
                    return StatusCode(403, "You can only delete your own quizzes.");

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

        // GET: api/quiz/my-quizzes
        [Authorize]
        [HttpGet("my-quizzes")]
        public async Task<IActionResult> GetMyQuizzes()
        {
            try
            {
                var (user, error) = await GetVerifiedTeacher();
                if (error != null) return error;

                var quizzes = await _context.Quizs
                    .Where(q => q.TeacherId == user!.Id)
                    .Include(q => q.Course)
                    .ToListAsync();

                if (!quizzes.Any())
                    return NotFound("No quizzes found for this teacher.");

                return Ok(new
                {
                    teacherId = user!.Id,
                    totalQuizzes = quizzes.Count,
                    quizzes = quizzes.Select(q => new
                    {
                        quizId = q.Quiz_Id,
                        title = q.Title,
                        totalMarks = q.Total_Marks,
                        question = q.Question,
                        courseId = q.Course_Id,
                        courseTitle = q.Course.Title
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
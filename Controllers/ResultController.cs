using learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Models;
using System.Security.Claims;

namespace practice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly AppDbContenxt _context;

        public ResultController(AppDbContenxt context)
        {
            _context = context;
        }

        // ==================== CREATE RESULT ====================
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResult([FromBody] Result model)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                // Assign StudentId from JWT
                model.Student_Id = studentId;

                // Check quiz exists
                var quiz = await _context.Quizs.FindAsync(model.Quiz_Id);
                if (quiz == null)
                    return NotFound("Quiz not found.");

                // Prevent duplicate submission
                var alreadySubmitted = await _context.Result
                    .AnyAsync(r => r.Quiz_Id == model.Quiz_Id && r.Student_Id == studentId);

                if (alreadySubmitted)
                    return BadRequest("You have already submitted this quiz.");

                await _context.Result.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Result submitted successfully",
                    resultId = model.Result_Id,
                    studentId = model.Student_Id,
                    quizId = model.Quiz_Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ==================== UPDATE RESULT ====================
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] Result model)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var result = await _context.Result.FindAsync(id);

                if (result == null)
                    return NotFound("Result not found.");

                // Ensure student owns the result
                if (result.Student_Id != studentId)
                    return Forbid("You can only update your own results.");

                // Validate quiz exists
                var quiz = await _context.Quizs.FindAsync(model.Quiz_Id);

                if (quiz == null)
                    return NotFound("Quiz not found.");

                result.Quiz_Id = model.Quiz_Id;
                result.Answer_Text = model.Answer_Text;
                result.Marks_Obtained = model.Marks_Obtained;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Result updated successfully",
                    resultId = result.Result_Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ==================== DELETE RESULT ====================
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var result = await _context.Result.FindAsync(id);

                if (result == null)
                    return NotFound("Result not found.");

                if (result.Student_Id != studentId)
                    return Forbid("You can only delete your own results.");

                _context.Result.Remove(result);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Result deleted successfully",
                    resultId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ==================== GET RESULTS OF LOGGED-IN STUDENT ====================
        [HttpGet("my-results")]
        public async Task<IActionResult> GetMyResults()
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (studentId == null)
                    return Unauthorized("Student ID not found in token.");

                var results = await _context.Result
                    .Where(r => r.Student_Id == studentId)
                    .Include(r => r.Quiz)
                    .ToListAsync();

                return Ok(results.Select(r => new
                {
                    resultId = r.Result_Id,
                    quizId = r.Quiz_Id,
                    quizTitle = r.Quiz.Title,
                    answerText = r.Answer_Text,
                    marksObtained = r.Marks_Obtained
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
//using Learnify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//using BCrypt.Net;
/*
namespace learnify.Controllers
{
    [ApiController]
    [Route("api/teacher")]
    public class teacherController : ControllerBase
    {
        private readonly ApplicationDbContenxt _context;
        public teacherController(AppDbContenxt context) { 
               _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Addteacher(Teacher teacher)
        {
            try
            {
                var existingTeacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == teacher.Email);

                if (existingTeacher != null)
                {
                    return BadRequest("Email already exists");
                }

                teacher.Password = BCrypt.Net.BCrypt.HashPassword(teacher.Password);

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                teacher.Password = string.Empty;

                return Ok(teacher);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}*/

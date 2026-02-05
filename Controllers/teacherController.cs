using learnify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace learnify.Controllers
{
    [ApiController]
    [Route("api/teacher")]
    public class teacherController : ControllerBase
    {
        private readonly AppDbContenxt _context;
        public teacherController(AppDbContenxt context) { 
               _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Addteacher(Teacher teacher)
        {
            try
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return Ok(teacher);
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}

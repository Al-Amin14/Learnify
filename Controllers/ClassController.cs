using learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly AppDbContenxt _context;

        public ClassesController(AppDbContenxt context)
        {
            _context = context;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] Classes model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _context.AddAsync(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Class created successfully",
                    classId = model.Classes_id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var cls = await _context.Set<Classes>().FindAsync(id);

            if (cls == null)
                return NotFound("Class not found");

            _context.Remove(cls);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Class deleted successfully" });
        }
    }
}
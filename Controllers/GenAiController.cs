using learnify.Models;
using Learnify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Learnify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenAiController : ControllerBase
    {
        private readonly AppDbContenxt _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly UserManager<Users> _userManager;

        public GenAiController(
            AppDbContenxt context,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            UserManager<Users> userManager)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
            _userManager = userManager;
        }

        // POST: api/GenAi/suggest
        [Authorize]
        [HttpPost("suggest")]
        public async Task<IActionResult> Suggest([FromBody] JsonElement body)
        {
            try
            {
                // 1. Get logged-in user's Id from JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Ok(new
                    {
                        reply = userId
                    });

                // 2. Verify user exists and is a Teacher
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Unauthorized("User not found.");

                // 3. Get question from request body
                if (!body.TryGetProperty("question", out var qElement))
                    return BadRequest("Question is required.");

                var question = qElement.GetString();

                if (string.IsNullOrWhiteSpace(question))
                    return BadRequest("Question is required.");

                // 4. Allowed questions
                var allowedQuestions = new List<string>
                {
                    "Which course should I choose?",
                    "What is the best course for beginners?",
                    "Recommend me a course based on my level.",
                    "Which course is most popular?",
                    "What course is best for getting a job?"
                };

                if (!allowedQuestions.Contains(question))
                {
                    return Ok(new
                    {
                        reply = "I can only answer the 5 fixed questions."
                    });
                }

                // 5. Fetch courses
                var courses = _context.Courses
                    .Select(c => new { c.Title, c.Description })
                    .ToList();

                // 6. Build context
                var contextText = "Available courses on Learnify platform:\n";
                foreach (var c in courses)
                {
                    contextText += $"- {c.Title}: {c.Description}\n";
                }

                // 7. Prepare Gemini request
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "model",
                            parts = new[] { new { text = contextText } }
                        },
                        new
                        {
                            role = "user",
                            parts = new[] { new { text = question } }
                        }
                    }
                };

                var apiKey = _config["AI:GeminiKey"];
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var json = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                // 8. Call Gemini API
                var response = await _httpClient.PostAsync(url, httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, responseBody);

                // 9. Parse response
                var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var text = data
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "No response from AI";

                // 10. Save interaction
                var interaction = new ApiInteraction
                {
                    UserId = user.Id, // from AspNetUsers
                    UserMessage = question,
                    ApiResponse = text,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ApiInteraction.AddAsync(interaction);
                await _context.SaveChangesAsync();

                // 11. Return response
                return Ok(new
                {
                    message = "AI response generated successfully",
                    reply = text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
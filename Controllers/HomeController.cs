using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Learnify.Models;

namespace Learnify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Learnify API is running");
        }

        [HttpGet("about")]
        public IActionResult AboutUs()
        {
            return Ok("This is Learnify platform API");
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return Ok("Privacy endpoint");
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return Problem("An error occurred");
        }
    }
}
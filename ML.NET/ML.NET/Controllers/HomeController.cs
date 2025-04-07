using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ML.NET.Models;
using Microsoft.Extensions.Logging;

namespace ML.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            //// 故意拋出例外來測試全域例外處理
            //throw new Exception("測試例外處理功能");
            _logger.LogInformation("進入 HomeController 的 Index Action");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

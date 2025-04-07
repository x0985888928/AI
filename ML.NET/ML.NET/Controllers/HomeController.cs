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

            //// �G�N�ߥX�ҥ~�Ӵ��ե���ҥ~�B�z
            //throw new Exception("���ըҥ~�B�z�\��");
            _logger.LogInformation("�i�J HomeController �� Index Action");
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

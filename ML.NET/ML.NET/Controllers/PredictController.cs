using Microsoft.AspNetCore.Mvc;
using ML.NET.Models;
using ML.NET.Services;

namespace ML.NET.Controllers
{
    public class PredictController : Controller
    {
        private readonly PricePredictionService _predictionService;

        public PredictController(PricePredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // 這裡可顯示一個表單頁面，讓使用者輸入參數
            return View();
        }

        [HttpPost]
        public IActionResult Predict(PricePredictionInput input)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            // 呼叫服務進行預測
            float predictedPrice = _predictionService.Predict(input);

            // 將結果帶到 View 顯示
            ViewBag.PredictedPrice = predictedPrice;
            return View("Index");
        }
    }
}

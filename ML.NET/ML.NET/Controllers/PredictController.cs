using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ML.NET.Models;
using ML.NET.Services;
using System.Runtime.Intrinsics.Arm;
using System;

namespace ML.NET.Controllers
{
    public class PredictController : Controller
    {
        private readonly PricePredictionService _predictionService;
        private readonly HistoricalPriceService _historyService;

        public PredictController(
            PricePredictionService predictionService,
            HistoricalPriceService historyService)
        {
            _predictionService = predictionService;
            _historyService = historyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // 1. 預設
            var model = new PricePredictionInput
            {
                CompanyName = "Apple",
                Brand = "iPhone",
                Version = "15",
                TYPE = "PRO",
                ROM = 128,
                MobileWeight = 200,
                RAM = 16,
                BatteryCapacity = 3000,
                LaunchedYear = 2025
            };

            // 2. 下拉選單來源
            ViewBag.Companies = new SelectList(new[] { "Apple", "Samsung", "Huawei" });
            ViewBag.Brands = new SelectList(new[] { "iPhone", "Galaxy S", "P Series" });
            ViewBag.Versions = new SelectList(new[] { "1", "2", "3", "14", "15", "16" });
            ViewBag.Types = new SelectList(new[] { "PRO", "PRO MAX" });

            return View(model);
        }

        [HttpPost]
        public IActionResult Predict(PricePredictionInput input)
        {
            // 1. 先重新填好下拉清單
            ViewBag.Companies = new SelectList(new[] { "Apple", "Samsung", "Huawei" }, input.CompanyName);
            ViewBag.Brands = new SelectList(new[] { "iPhone", "Galaxy S", "P Series" }, input.Brand);
            ViewBag.Versions = new SelectList(new[] { "1", "2", "3", "14", "15", "16" }, input.Version);
            ViewBag.Types = new SelectList(new[] { "PRO", "PRO MAX" }, input.TYPE);

            // 2. 如果驗證失敗，直接回原本頁面，ModelState 會顯示錯誤
            if (!ModelState.IsValid)
            {
                return View("Index", input);
            }

            // 3. 呼叫預測
            var predPrice = _predictionService.Predict(input);
            ViewBag.PredictedPrice = predPrice;
            ViewBag.PredictedYear = input.LaunchedYear;   // 傳年份給前端
            ViewBag.PredictedType = input.TYPE;           // 該條 Type 名稱

            // 3. 取得歷史資料
            var history = _historyService.Get(
                input.CompanyName ?? "",
                input.Brand ?? "",
                input.Version ?? "",
                input.TYPE ?? "",
                input.ROM,
                input.MobileWeight,
                input.RAM,
                input.BatteryCapacity,
                input.LaunchedYear);
            // 4. 傳給 View（序列化成 JSON）
            ViewBag.HistoryJson = System.Text.Json.JsonSerializer.Serialize(history);

            // 4. 回傳同一個 View 並帶上 input
            return View("Index", input);
        }
    }
}

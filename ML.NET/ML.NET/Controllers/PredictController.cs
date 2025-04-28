using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ML.NET.Models;
using ML.NET.Services;
using System.Runtime.Intrinsics.Arm;

public class PredictController : Controller
{
    private readonly PricePredictionService _predictSvc;
    private readonly HistoricalPriceService _historySvc;
    private readonly PhoneOptions _phoneOptions;

    public PredictController(
        PricePredictionService predictSvc,
        HistoricalPriceService historySvc,
        IOptions<PhoneOptions> phoneOptions)
    {
        _predictSvc = predictSvc;
        _historySvc = historySvc;
        _phoneOptions = phoneOptions.Value;
    }

    // ---------- GET ----------
    public IActionResult Index()
    {
        var model = new PricePredictionInput { /* 預設值 */ };
        // PredictController.cs（GET 與 POST 都加）
        ViewBag.PhoneOptionsJson =
            System.Text.Json.JsonSerializer.Serialize(_phoneOptions);

        // Company 下拉 (全部公司)
        ViewBag.Companies = new SelectList(
            _phoneOptions.Companies.Select(c => c.Name));

        // 先用第一家公司填品牌 & 型號
        var firstCompany = _phoneOptions.Companies.First();
        ViewBag.Brands = new SelectList(firstCompany.Brands.Select(b => b.Name));
        ViewBag.Types = new SelectList(firstCompany.Brands.First().Types);

        return View(model);
    }

    // ---------- POST ----------
    [HttpPost]
    public IActionResult Predict(PricePredictionInput input)
    {
        // PredictController.cs（GET 與 POST 都加）
        ViewBag.PhoneOptionsJson =
            System.Text.Json.JsonSerializer.Serialize(_phoneOptions);

        /* 1) 重新填下拉清單 —— 靜態來源，不打 DB */
        ViewBag.Companies = new SelectList(
            _phoneOptions.Companies.Select(c => c.Name), input.CompanyName);

        var brands = _phoneOptions.Companies
            .FirstOrDefault(c => c.Name == input.CompanyName)
            ?.Brands ?? new List<Brand>();

        ViewBag.Brands = new SelectList(
            brands.Select(b => b.Name), input.Brand);

        var types = brands
            .FirstOrDefault(b => b.Name == input.Brand)
            ?.Types ?? new List<string>();

        ViewBag.Types = new SelectList(types, input.TYPE);

        /* 2) 做預測 & 歷史資料（與之前相同） */
        var predPrice = _predictSvc.Predict(input);
        ViewBag.PredictedPrice = predPrice;
        ViewBag.PredictedYear = input.LaunchedYear;
        ViewBag.PredictedType = input.TYPE;
        var history = _historySvc.GetByCompany(input.CompanyName ?? "",
                input.Brand ?? "",
                input.Version ?? "",
                input.TYPE ?? "",
                input.ROM,
                input.MobileWeight,
                input.RAM,
                input.BatteryCapacity,
                input.LaunchedYear);
        ViewBag.HistoryJson = System.Text.Json.JsonSerializer.Serialize(history);

        return View("Index", input);
    }
}

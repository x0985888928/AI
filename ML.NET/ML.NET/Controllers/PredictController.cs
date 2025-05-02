using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ML.NET.Models;
using ML.NET.Services;
using System.Text.Json;

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

    // GET /Predict
    public IActionResult Index()
    {
        ViewBag.PhoneOptionsJson = JsonSerializer.Serialize(_phoneOptions);
        // Companies
        ViewBag.Companies = new SelectList(_phoneOptions.Companies.Select(c => c.Name));

        // 先拿第一家
        var first = _phoneOptions.Companies.First();
        // Brands
        ViewBag.Brands = new SelectList(first.Brands.Select(b => b.Name));

        // Versions：取第一家、第一個 Brand 下的 Versions
        ViewBag.Versions = new SelectList(
            first.Brands.First().Versions
        );

        // Types
        ViewBag.Types = new SelectList(
            first.Brands.First().Types
        );

        return View(new PricePredictionInput());
    }

    [HttpPost]
    public IActionResult Predict(PricePredictionInput input)
    {
        ViewBag.PhoneOptionsJson = JsonSerializer.Serialize(_phoneOptions);
        ViewBag.Companies = new SelectList(
            _phoneOptions.Companies.Select(c => c.Name),
            input.CompanyName
        );

        var company = _phoneOptions.Companies
                         .First(c => c.Name == input.CompanyName);
        ViewBag.Brands = new SelectList(
            company.Brands.Select(b => b.Name),
            input.Brand
        );

        var brand = company.Brands
                           .First(b => b.Name == input.Brand);
        // **分開 Versions 與 Types**
        ViewBag.Versions = new SelectList(
            brand.Versions,   // 這裡用 Versions
            input.Version
        );
        ViewBag.Types = new SelectList(
            brand.Types,
            input.TYPE
        );

        // 預測與歷史同原本
        ViewBag.PredictedPrice = _predictSvc.Predict(input);
        var history = _historySvc.GetByCompany(
          input.CompanyName, input.Brand, input.Version, input.TYPE,
          input.ROM, input.MobileWeight, input.RAM,
          input.BatteryCapacity, input.LaunchedYear);
        ViewBag.HistoryJson = JsonSerializer.Serialize(history);

        return View("Index", input);
    }

}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ML.NET.Models
{
    public class PricePredictionInput
    {
        public string CompanyName { get; set; } = default!;
        public string Brand { get; set; } = default!;
        public string Type { get; set; } = default!;
        public float ROM { get; set; }
        public float MobileWeight { get; set; }
        public float RAM { get; set; }
        public float BatteryCapacity { get; set; }
        public float LaunchedYear { get; set; }
    }
}

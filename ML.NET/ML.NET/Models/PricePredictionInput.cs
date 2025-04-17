using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Data;

namespace ML.NET.Models
{
    public class PricePredictionInput
    {
        public string? CompanyName { get; set; } = default!;

        public string? Brand { get; set; } = default!;

        public string? Version { get; set; } = default!;

        public string? TYPE { get; set; } = default!;

        public float ROM { get; set; } = default!;

        public float MobileWeight { get; set; } = default!;

        public float RAM { get; set; } 

        public float BatteryCapacity { get; set; }

        public float LaunchedYear { get; set; }

    }
}

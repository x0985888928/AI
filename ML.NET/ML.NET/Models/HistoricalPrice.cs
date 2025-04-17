namespace ML.NET.Models
{
    public class HistoricalPrice
    {
        public string CompanyName { get; set; } = default!;

        public string Brand { get; set; } = default!;

        public string? Version { get; set; } = default!;

        public string Type { get; set; } = default!;

        public float ROM { get; set; } = default!;

        public float MobileWeight { get; set; } = default!;

        public float RAM { get; set; }

        public float BatteryCapacity { get; set; }

        public float LaunchedYear { get; set; }

        public float Price { get; set; }
    }
}

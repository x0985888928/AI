using ML.NET.Models;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

namespace ML.NET.Services
{
    public class HistoricalPriceService
    {
        private readonly List<HistoricalPrice> _data;

        public HistoricalPriceService(IWebHostEnvironment env)
        {
            // 1. 讀取 CSV 全部行
            var path = Path.Combine(env.ContentRootPath, "Data", "historical_prices.csv");
            var lines = File.ReadAllLines(path).Skip(1); // 跳過標頭

            // 2. 解析並存成 list
            _data = lines
                .Select(line =>
                {
                    var parts = line.Split(',')
                        .Select(f => f.Trim().Trim('"'))
                        .ToArray();
                    return new HistoricalPrice
                    {
                        CompanyName = parts[0],
                        Brand = parts[1],
                        Version = parts[2],
                        Type = parts[3],
                        ROM = float.Parse(parts[4], CultureInfo.InvariantCulture),
                        MobileWeight = float.Parse(parts[5], CultureInfo.InvariantCulture),
                        RAM = float.Parse(parts[6], CultureInfo.InvariantCulture),
                        BatteryCapacity = float.Parse(parts[7], CultureInfo.InvariantCulture),
                        LaunchedYear = float.Parse(parts[8], CultureInfo.InvariantCulture),
                        Price = float.Parse(parts[9], CultureInfo.InvariantCulture)
                    };
                })
                .ToList();
        }

        // 根據使用者輸入過濾出該品牌／機種的歷史資料
        public List<HistoricalPrice> Get(string company, string brand,string Version, string type, float ROM,float MobileWeight,float RAM,float BatteryCapacity,float LaunchedYear)
        {
            return _data
                .Where(x =>
                    x.CompanyName.Equals(company, StringComparison.OrdinalIgnoreCase)// &&
                    //x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
                    //x.Type.Equals(type, StringComparison.OrdinalIgnoreCase) && 
                    //x.ROM.Equals(ROM) &&
                    //x.MobileWeight.Equals(MobileWeight) &&
                    //x.RAM.Equals(RAM) &&
                    //x.BatteryCapacity.Equals(BatteryCapacity) &&
                    //x.LaunchedYear.Equals(LaunchedYear)
                )
                .OrderBy(x => x.LaunchedYear)
                .ToList();
        }
    }
}

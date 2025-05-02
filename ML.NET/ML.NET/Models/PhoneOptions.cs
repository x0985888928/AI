// Models/PhoneOptions.cs
namespace ML.NET.Models
{
    public class PhoneOptions
    {
        public List<Company> Companies { get; set; } = new();
    }

    public class Company
    {
        public string Name { get; set; } = default!;
        public List<Brand> Brands { get; set; } = new();
    }

    public class Brand
    {
        public string Name { get; set; } = default!;
        /// <summary>
        /// 例如同一品牌不同機型版本
        /// </summary>
        public List<string> Versions { get; set; } = new();

        /// <summary>
        /// 原本的 TYPE 列表
        /// </summary>
        public List<string> Types { get; set; } = new();
    }
}

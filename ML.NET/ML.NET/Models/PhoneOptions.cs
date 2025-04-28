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
        public List<string> Types { get; set; } = new();
    }
}

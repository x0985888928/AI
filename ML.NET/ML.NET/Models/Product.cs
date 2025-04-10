using System.ComponentModel.DataAnnotations;

namespace ML.NET.Models
{
    public class Product
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "請輸入產品名稱")] 
        public string? Name { get; set; } = default!;
        [Range(0.0, double.MaxValue, ErrorMessage = "價格必須為非負數")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "庫存數量必須為非負數")]
        public int Stock { get; set; }
    }
}
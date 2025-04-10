using System.ComponentModel.DataAnnotations;

namespace ML.NET.Models
{
    public class Customer
    {
        [Key]
        public int FormNo { get; set; }
        [Required]
        public string UserID { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string ProductNum { get; set; } = default!;
    }
}

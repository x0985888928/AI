using System.ComponentModel.DataAnnotations;

namespace ML.NET.Models
{
    public class ContactFormModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required, StringLength(500)]
        public string Message { get; set; }
    }
}

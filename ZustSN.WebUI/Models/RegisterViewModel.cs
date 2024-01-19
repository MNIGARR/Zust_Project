using System.ComponentModel.DataAnnotations;

namespace ZustSN.WebUI.Models
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Please enter a password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }       
        [Display(Name = "I accept the privacy policy")]
        public bool AcceptPrivacy { get; set; }
        public IFormFile? File { get; set; }
        public string? ImageUrl { get; set; } = "user-1.jpg";
    }
}

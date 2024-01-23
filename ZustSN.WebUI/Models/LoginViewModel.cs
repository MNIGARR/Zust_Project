using System.ComponentModel.DataAnnotations;


namespace ZustSN.WebUI.Models
{
    public class LoginViewModel
    {

        [Required (ErrorMessage ="Please complete this field correctly.")]
        public string? Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

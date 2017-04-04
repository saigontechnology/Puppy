using System.ComponentModel.DataAnnotations;

namespace TopCore.SSO.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
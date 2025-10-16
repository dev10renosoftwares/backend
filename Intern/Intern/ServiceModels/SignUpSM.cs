using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Intern.DataModels.Enums;

namespace Intern.ServiceModels
{
    public class SignUpSM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DefaultValue("string")]  
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DefaultValue("string")]
        public string ConfirmPassword { get; set; }

        
        public string MobileNumber { get; set; }

        [DefaultValue("string")]
        public string? ImagePath { get; set; }



       
    }
}

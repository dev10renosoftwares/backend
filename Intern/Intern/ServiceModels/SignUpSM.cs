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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,20}$",
            ErrorMessage = "Password must contain upper, lower, digit, and special character.")]
        [DefaultValue("string")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DefaultValue("string")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [DefaultValue("string")]
        public string MobileNumber { get; set; }

        [DefaultValue("string")]
        public string? ImagePath { get; set; }



       
    }
}

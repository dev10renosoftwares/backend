using System.ComponentModel.DataAnnotations;
using Intern.DataModels.BaseDataModels;
using Intern.DataModels.Enums;

namespace Intern.DataModels.User
{
    public class BaseUserDM : BaseDM
    {
        
        [Required(ErrorMessage = "LoginId is required")]
        [MaxLength(50, ErrorMessage = "LoginId cannot exceed 50 characters")]
        public string LoginId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRoleDM Role { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        [MaxLength(15, ErrorMessage = "Mobile number cannot exceed 15 digits")]
        public string? MobileNumber { get; set; }

        public string? ImagePath { get; set; }




    }
}

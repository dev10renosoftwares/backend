using Intern.DataModels.Enums;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class ClientUserSM : BaseSM
    {

        [Required(ErrorMessage = "LoginId is required")]
        [MaxLength(50, ErrorMessage = "LoginId cannot exceed 50 characters")]
        public string UserName { get; set; }

        
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
        public string? Password { get; set; }

        // ✅ No max length restriction — will map to nvarchar(max) in SQL
        public string? ImageBase64 { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRoleSM Role { get; set; }   // Admin, SuperAdmin, etc.

        
        public string? MobileNumber { get; set; }

        public bool IsActive { get; set; } 

        public bool IsEmailConfirmed { get; set; } 

        public bool IsMobileNumberConfirmed { get; set; }


       


    }
}

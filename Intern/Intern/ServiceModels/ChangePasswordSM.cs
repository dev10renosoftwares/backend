using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class ChangePasswordSM
    {
        [Required(ErrorMessage = "Old password is required.")]
        [MinLength(6, ErrorMessage = "Old password must be at least 6 characters.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Confirm password does not match new password.")]
        public string ConfirmPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class ResetPasswordSM
    {
        [Required(ErrorMessage ="Newpassword is required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Authcode is required")]
        public string AuthCode { get; set; }
    }
}

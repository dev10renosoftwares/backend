using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class VerifyEmailSM
    {
        [Required]
        public string Token { get; set; }
    }
}

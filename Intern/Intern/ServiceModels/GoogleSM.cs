using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class GoogleSM
    {
        [Required(ErrorMessage = "IdToken is required.")]
        public string IdToken { get; set; }

        [Required(ErrorMessage ="RefreshToken is required")]
        public string RefreshToken { get; set; }

        [Required(ErrorMessage = "IsLogin flag is required.")]
        public bool IsLogin { get; set; }
    }

}

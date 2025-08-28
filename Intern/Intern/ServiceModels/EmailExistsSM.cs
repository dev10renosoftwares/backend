using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels
{
    public class EmailExistsSM
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DefaultValue("string")]
        public string Email { get; set; }
    }
}

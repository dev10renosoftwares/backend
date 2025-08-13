using System.ComponentModel.DataAnnotations;

namespace Intern.DataModels
{
    public class UserDM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string LoginId { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}

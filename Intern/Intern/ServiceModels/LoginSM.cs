using Intern.DataModels.Enums;

namespace Intern.ServiceModels
{
    public class LoginSM
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public UserRoleDM Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Intern.DataModels.Enums;

namespace Intern.ServiceModels
{
    public class LoginResponseSM
    {

        public int Id { get; set; }

        public string LoginId {  get; set; }
        
        public string Email { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public UserRoleDM Role { get; set; }

        public string? MobileNumber { get; set; }

        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Intern.DataModels.Enums;

namespace Intern.ServiceModels
{
    public class UserSM
    {
        public int Id { get; set; }
        
      
   
        public string LoginId { get; set; }

       
        public string Email { get; set; }

        public string? ImagePath { get; set; }

        public string? Password { get; set; }

      
        public UserRoleDM Role { get; set; }

        public string? MobileNumber { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOnUtc { get; set; }
        public string? LastModifiedBy { get; set; }

    }
}

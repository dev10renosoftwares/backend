using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Intern.DataModels.Enums;
using Intern.DataModels.BaseDataModels;

namespace Intern.DataModels.User
{
    public class ExternalUserDM : BaseDM
    {   
        public LoginTypeDM  LoginType { get; set; }  

        public string RefreshToken { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ClientUserDM User { get; set; }
    }


}

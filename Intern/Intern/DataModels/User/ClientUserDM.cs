namespace Intern.DataModels.User
{
    public class ClientUserDM : BaseUserDM
    {       
        
    
       
        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsMobileNumberConfirmed { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public ICollection<ExternalUserDM> ExternalUsers { get; set; }
    }
}



    




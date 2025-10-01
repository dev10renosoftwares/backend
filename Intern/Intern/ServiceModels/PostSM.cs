using Intern.ServiceModels.BaseServiceModels;

namespace Intern.ServiceModels
{
    public class PostSM : BaseSM
    {


        public string PostName { get; set; }
        public string Description { get; set; }
        public DateTime? PostDate { get; set; }

        public string? NotificationNumber { get; set; }

    }
  
}

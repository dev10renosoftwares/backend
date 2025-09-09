using Intern.ServiceModels.BaseServiceModels;

namespace Intern.ServiceModels
{
    public class PostSM : BaseSM
    {


        public string PostName { get; set; }
        public string Description { get; set; }

    }


    public class AddPostSM
    {
        public string? PostName { get; set; }
        public string? Description { get; set; }
    }
}

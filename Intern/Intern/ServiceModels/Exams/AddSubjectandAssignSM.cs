using Intern.ServiceModels.BaseServiceModels;

namespace Intern.ServiceModels.Exams
{
    public class AddSubjectandAssignSM :BaseSM
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }

        public int PostId { get; set;}

    }
}

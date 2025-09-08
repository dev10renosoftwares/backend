using Intern.DataModels.BaseDataModels;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class SubjectDM : BaseDM
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }

        public ICollection<SubjectPostDM> SubjectPosts { get; set; }
    }
}

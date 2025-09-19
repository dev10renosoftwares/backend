using Intern.DataModels.BaseDataModels;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class PostDM : BaseDM
    {
        public string PostName { get; set; }
        public string Description { get; set; }
        public ICollection<DepartmentPostsDM> DepartmentPosts { get; set; }
        public ICollection<SubjectPostDM> SubjectPosts { get; set; }

        public ICollection<MCQPostSubjectDM> MCQPostSubjects { get; set; }
    }
}

using Intern.DataModels.BaseDataModels;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class DepartmentDM : BaseDM
    {
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public ICollection<DepartmentPostsDM> DepartmentPosts { get; set; }
    }
}

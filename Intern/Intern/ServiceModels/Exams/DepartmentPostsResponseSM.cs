using Intern.DataModels.Exams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.ServiceModels.Exams
{
    public class DepartmentPostsResponseSM
    {
        public DepartmentSM Department { get; set; }

        public List<DepartmentPostRelationSM> Posts { get; set; }
    }
}

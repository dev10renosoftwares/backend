using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.DataModels.Exams
{
    public class DepartmentPostsDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(DepartmentDM))]
        public int? DepartmentId { get; set; }
        public DepartmentDM? Department { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? PostId { get; set; }
        public PostDM Post { get; set; }

        public DateTime PostDate { get; set; }

        public string? NotificationNumber { get; set; }
        public ICollection<MCQPostSubjectDM> MCQPostSubjects { get; set; }

    }
}

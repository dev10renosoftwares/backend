using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intern.DataModels.Enums;

namespace Intern.DataModels.Exams
{
    public class MCQPostSubjectDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(SubjectPostDM))]
        public int? SubjectPostId { get; set; }
        public SubjectPostDM? SubjectPost { get; set; }

        [ForeignKey(nameof(DepartmentPostsDM))]
        public int? DepartmentPostId { get; set; }
        public DepartmentPostsDM? DepartmentPosts { get; set; }

        [ForeignKey(nameof(MCQsDM))]
        public int? MCQId { get; set; }
        public MCQsDM? MCQ { get; set; }

        public McqTypeDM MCQType { get; set; }
    }
}

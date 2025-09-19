using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intern.DataModels.Enums;

namespace Intern.DataModels.Exams
{
    public class MCQPostSubjectDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(SubjectDM))]
        public int? SubjectId { get; set; }
        public SubjectDM? Subject { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? PostId { get; set; }
        public PostDM? Post { get; set; }

        [ForeignKey(nameof(MCQsDM))]
        public int? MCQId { get; set; }
        public MCQsDM? MCQ { get; set; }

        public McqTypeDM MCQType { get; set; }
    }
}

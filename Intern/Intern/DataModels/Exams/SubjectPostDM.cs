using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.DataModels.Exams
{
    public class SubjectPostDM 
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(SubjectDM))]
        public int? SubjectId { get; set; }
        public SubjectDM Subject { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? PostId { get; set; }
        public PostDM Post { get; set; }

        public ICollection<MCQPostSubjectDM> MCQPostSubjects { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.DataModels.Exams
{
    public class PostPapersDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(DepartmentDM))]
        public int? PostId { get; set; }
        public PostDM Post { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? PaperId { get; set; }
        public PapersDM PreviousYearPapers { get; set; }

        public DateTime ExamYear { get; set; }


    }
}

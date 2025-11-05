using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.DataModels.Exams
{
    public class PostPapersDM
    {
            [Key]
            public int Id { get; set; }

            // Foreign key to Post (nullable in SQL)
            public int? PostId { get; set; }

            [ForeignKey(nameof(PostId))]
            public PostDM Post { get; set; }

            // Foreign key to Previous Year Paper
            [Required]
            public int PreviousYearPapersId { get; set; }

            [ForeignKey(nameof(PreviousYearPapersId))]
            public PapersDM PreviousYearPaper { get; set; }

            [Required]
            public DateTime ExamYear { get; set; }
        
    }

}

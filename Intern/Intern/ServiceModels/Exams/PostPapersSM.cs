using Intern.DataModels.Exams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.ServiceModels.Exams
{
    public class PostPapersSM
    {
       
        public int PostId { get; set; }
      
        public int PreviousYearPapersId { get; set; }

        public DateTime ExamYear { get; set; }
    }
}

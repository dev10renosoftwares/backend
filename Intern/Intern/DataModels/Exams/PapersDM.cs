using System.ComponentModel.DataAnnotations;
using Intern.DataModels.BaseDataModels;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class PapersDM : BaseDM
    {
        [Required]
        public string PaperTitle { get; set; }  

        public string Description { get; set; }  

        public string FilePath { get; set; }    
 

        // 🔁 Navigation Property (many-to-many via PostPapersDM)
        public ICollection<PostPapersDM> PostPapers { get; set; }
    }
}

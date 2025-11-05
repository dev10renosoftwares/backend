using System.ComponentModel.DataAnnotations;
using Intern.DataModels.BaseDataModels;

namespace Intern.DataModels.Exams
{
    public class SyllabusDM : BaseDM
    {
    

        [Required]
        public string Title { get; set; }   

        public string Description { get; set; }  

        public string FilePath { get; set; }  
        // 🔁 Navigation Property (many-to-many via PostSyllabusDM)
        public ICollection<PostSyllabusDM> PostSyllabuses { get; set; }

    }
}

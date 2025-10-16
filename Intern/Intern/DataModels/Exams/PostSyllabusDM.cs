using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.DataModels.Exams
{
    public class PostSyllabusDM
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(DepartmentDM))]
        public int? PostId { get; set; }
        public PostDM Post { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? SyllabusId { get; set; }
        public SyllabusDM Syllabus { get; set; }

        public DateTime YearOfExam { get; set; }


    }
}

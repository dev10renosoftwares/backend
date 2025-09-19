using Intern.DataModels.Enums;
using Intern.ServiceModels.Enums;

namespace Intern.ServiceModels.Exams
{
    public class MCQSubjectPostSM
    {
        public int Id { get; set; }

        public int? SubjectId { get; set; }

        public int? PostId { get; set; }

        public int MCQId { get; set; }

        public McqTypeSM McqType { get; set; }
    }
}

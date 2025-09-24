using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Enums;

namespace Intern.ServiceModels.User
{
    public class UserTestDetailsSM:BaseSM
    {
        public int UserId { get; set; }
        public int PostId { get; set; }

        public int? SubjectId { get; set; }

        public McqTypeSM MCQType { get; set; }

        public int TotalQuestions { get; set; }
        public int NotAttempted { get; set; } 

        public int RightAnswered { get; set; }
        public int WrongAnswered { get; set; }

        public bool TestTaken { get; set; }
        public bool TestSubmitted { get; set; }
    }
}

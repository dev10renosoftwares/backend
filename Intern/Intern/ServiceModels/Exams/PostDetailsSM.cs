using Intern.ServiceModels.User;

namespace Intern.ServiceModels.Exams
{
    public class PostDetailsSM
    {
        public PostSM Post { get; set; }

        public List<SyllabusSM>  Syllabus {get; set;}

        public List<PapersSM> PreviousYearPapers { get; set; }

        public List<UserTestPerformanceSM> UserPerformance { get; set; }

        public List<NotificationsSM> Notifications { get; set; }
    }
}

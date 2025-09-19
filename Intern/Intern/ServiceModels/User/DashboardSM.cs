using Intern.ServiceModels.Exams;

namespace Intern.ServiceModels.User
{
    public class DashboardSM
    {       
        public List<PostSM> Exams { get; set; }// Department posts by deparmentId 

        public List<PostSM> UpcomingExams { get; set; }  

        public List<PostSM> RecommendedExams { get; set; }

        public List<NotificationsSM> Notifications { get; set; }

    }
}

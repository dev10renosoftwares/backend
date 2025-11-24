using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.ServiceModels.BaseServiceModels;
using Intern.ServiceModels.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.ServiceModels.Exams
{
    public class NotificationsSM : BaseSM
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationTypeSM NotificationType { get; set; }

        public int DepartmentId { get; set; }
        
        public int PostId { get; set; }
    }
}

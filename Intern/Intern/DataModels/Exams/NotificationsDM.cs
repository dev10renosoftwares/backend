using System.ComponentModel.DataAnnotations.Schema;
using Intern.DataModels.BaseDataModels;
using Intern.DataModels.Enums;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class NotificationsDM : BaseDM
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationTypeDM NotificationType { get; set; }

        [ForeignKey(nameof(DepartmentDM))]
        public int DepartmentId { get; set; }
        public DepartmentDM? Department { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int PostId { get; set; }
        public PostDM? Post { get; set; }

    }
}

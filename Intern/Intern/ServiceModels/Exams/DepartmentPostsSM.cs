using Intern.DataModels.Exams;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intern.ServiceModels.Exams
{
    public class DepartmentPostsSM
    {
        public int Id { get; set; }

        public int? DepartmentId { get; set; }

        public int? PostId { get; set; }

        public DateTime PostDate { get; set; }

        public string? NotificationNumber { get; set; }
    }
}

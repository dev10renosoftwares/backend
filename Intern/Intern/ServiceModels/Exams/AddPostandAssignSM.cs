using System.ComponentModel.DataAnnotations;

namespace Intern.ServiceModels.Exams
{
   
        public class AddPostandAssignSM
        {
            [Required(ErrorMessage = "Post name is required.")]
           
            public string PostName { get; set; }


          [Required(ErrorMessage = "Description  is required.")]
            public string Description { get; set; }

           [Required(ErrorMessage = "DepartmentId is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than 0.")]
           public int DepartmentId { get; set; }

            [Required(ErrorMessage = "PostDate is required.")]
            [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
            public DateTime PostDate { get; set; }

            [Required(ErrorMessage = "Notification number is required.")]
         
            public string NotificationNumber { get; set; }
        }
    
}

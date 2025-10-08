using System.ComponentModel.DataAnnotations.Schema;
using Intern.DataModels.BaseDataModels;
using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.ServiceModels;
using Intern.ServiceModels.Enums;

namespace Intern.DataModels.User
{
    public class UserTestDetailsDM: BaseDM
        
    {
        [ForeignKey(nameof(ClientUserDM))]
        public int UserId { get; set; }
        public ClientUserDM User { get; set; }

        [ForeignKey(nameof(PostDM))]
        public int? PostId { get; set; }
        public PostDM? Post { get; set; }


        [ForeignKey(nameof(SubjectDM))]
        public int? SubjectId { get; set; }
        public SubjectDM? Subject { get; set; }

        public McqTypeDM MCQType { get; set; }    
        public int TotalQuestions { get; set; }
           
        public int NotAttempted { get; set; }
        public int RightAnswered { get; set; }
        public int WrongAnswered { get; set; }

       
        public bool TestTaken { get; set; }   
        public bool TestSubmitted { get; set; } 
    }
}

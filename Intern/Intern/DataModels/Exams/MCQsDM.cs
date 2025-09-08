using Intern.DataModels.BaseDataModels;
using Intern.DataModels.User;

namespace Intern.DataModels.Exams
{
    public class MCQsDM : BaseDM
    {
        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        public string Answer { get; set; }

        public string Explanation { get; set; }

        public ICollection<MCQPostSubjectDM> MCQPostSubjects { get; set; }
    }
}

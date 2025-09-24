using Intern.ServiceModels.BaseServiceModels;

namespace Intern.ServiceModels.Exams
{
    public class MCQsSM:BaseSM
    {

        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        public string? Answer { get; set; }

        public string? Explanation { get; set; }
    }
}

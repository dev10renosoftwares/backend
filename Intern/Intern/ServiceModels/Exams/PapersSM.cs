using Intern.ServiceModels.BaseServiceModels;

namespace Intern.ServiceModels.Exams
{
    public class PapersSM : BaseSM
    {
        public string PaperTitle { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

    }
}

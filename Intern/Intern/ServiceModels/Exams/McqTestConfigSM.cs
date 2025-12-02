namespace Intern.ServiceModels.Exams
{
    public class McqTestConfigSM
    {
      
        public double NegativeMarkPerQuestion { get; set; }
        public double MarksPerQuestion { get; set; }
        public int TotalQuestionsPerMockTest { get; set; }
        public int TotalTimePerMockTest { get; set; }

    }
}

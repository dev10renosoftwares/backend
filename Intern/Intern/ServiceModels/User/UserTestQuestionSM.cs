namespace Intern.ServiceModels.User
{
    public class UserTestQuestionSM
    {
       public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string? UserAnswer { get; set; }       
        public string ActualAnswer { get; set; }
        public bool? IsUserAnswerCorrect { get; set; } 

    }
}

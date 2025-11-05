namespace Intern.ServiceModels.User
{
    public class UserTestPerformanceSM
    {
        
            public int PostId { get; set; }
            public int TotalQuestions { get; set; }
            public int NotAttempted { get; set; }
            public int RightAnswered { get; set; }
            public int WrongAnswered { get; set; }
            public bool TestTaken { get; set; }
            public bool TestSubmitted { get; set; }
            public DateTime TestTakenOnDate { get; set; }
        

    }
}

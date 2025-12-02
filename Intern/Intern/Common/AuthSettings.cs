namespace Intern.Common
{
    public class AuthSettings
    {
        public int EmailVerificationExpiryMinutes { get; set; }
        public int ResetPasswordExpiryMinutes { get; set; }
    }

    public class ExamConfig
    {
        public int MaxTestsPerUser { get; set; }

        public double NegativeMarkPerQuestion { get; set; }
        public double MarksPerQuestion { get; set; }
        public int TotalQuestionsPerMockTest { get; set; }
        public int TotalTimePerMockTest { get; set; }
    }
}

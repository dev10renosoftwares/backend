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
    }
}

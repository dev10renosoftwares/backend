namespace Intern.ServiceModels
{
    public class ResetPasswordPayloadSM
    {
        public string Email { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

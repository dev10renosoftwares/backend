namespace Intern.ServiceModels
{
    public class ChangePasswordSM
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}

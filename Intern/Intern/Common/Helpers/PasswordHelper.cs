using System.Security.Cryptography;
using System.Text;

namespace Intern.Common.Helpers
{
    public class PasswordHelper
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + "qwasdfghjdfghjkghjk%ergf%ghsdfxc*gvhbjnk@#";
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {

                var saltedPassword = password + "qwasdfghjdfghjkghjk%ergf%ghsdfxc*gvhbjnk@#";
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                var hashedInputPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedInputPassword == hashedPassword;
            }
        }
    }
}

using System.IdentityModel.Tokens.Jwt;

namespace Intern.Common.Helpers
{
    public class TokenHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private const string AuthorizationHeader = "Authorization";
        private const string BearerPrefix = "Bearer ";
        private const string UserIdClaim = "UserId";
        private const string EmailClaim = "Email";
        private const string RoleClaim = "Role";
        private const string LoginIdClaim = "LoginId";

        public TokenHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public int GetUserIdFromToken() => GetIntClaimFromToken(UserIdClaim);
        public string GetEmailFromToken() => GetStringClaimFromToken(EmailClaim);
        public int GetRoleFromToken() => GetIntClaimFromToken(RoleClaim);
        public string GetLoginIdFromToken() => GetStringClaimFromToken(LoginIdClaim);

        private int GetIntClaimFromToken(string claimType)
        {
            var token = GetJwtToken();
            var claimValue = token?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            if (string.IsNullOrWhiteSpace(claimValue))
                throw new InvalidOperationException($"Claim '{claimType}' is missing from token.");

            if (!int.TryParse(claimValue, out int result))
                throw new InvalidOperationException($"Claim '{claimType}' is not a valid integer.");

            return result;
        }

        private string GetStringClaimFromToken(string claimType)
        {
            var token = GetJwtToken();
            var claimValue = token?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            if (string.IsNullOrWhiteSpace(claimValue))
                throw new InvalidOperationException($"Claim '{claimType}' is missing from token.");

            return claimValue;
        }

        private JwtSecurityToken GetJwtToken()
        {
            var authHeader = _contextAccessor.HttpContext?.Request.Headers[AuthorizationHeader].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Authorization header is missing or invalid.");

            var tokenStr = authHeader.Substring(BearerPrefix.Length).Trim();
            if (string.IsNullOrWhiteSpace(tokenStr) || tokenStr.Split('.').Length != 3)
                throw new InvalidOperationException("JWT token format is invalid.");

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(tokenStr))
                throw new InvalidOperationException("Invalid JWT token.");

            return handler.ReadToken(tokenStr) as JwtSecurityToken
                   ?? throw new InvalidOperationException("Unable to read JWT token.");
        }
    }
}

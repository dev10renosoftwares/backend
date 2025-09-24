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

        public int? GetUserIdFromToken() => GetIntClaimFromToken(UserIdClaim);
        public string? GetEmailFromToken() => GetStringClaimFromToken(EmailClaim);
        public int? GetRoleFromToken() => GetIntClaimFromToken(RoleClaim);
        public string? GetLoginIdFromToken() => GetStringClaimFromToken(LoginIdClaim);

        private int? GetIntClaimFromToken(string claimType)
        {
            var token = GetJwtToken();
            if (token == null) return null;

            var claimValue = token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            return int.TryParse(claimValue, out int result) ? result : null;
        }

        private string? GetStringClaimFromToken(string claimType)
        {
            var token = GetJwtToken();
            return token?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        private JwtSecurityToken? GetJwtToken()
        {
            var authHeader = _contextAccessor.HttpContext?.Request.Headers[AuthorizationHeader].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            var tokenStr = authHeader.Substring(BearerPrefix.Length).Trim();
            if (string.IsNullOrWhiteSpace(tokenStr) || tokenStr.Split('.').Length != 3)
                return null;

            var handler = new JwtSecurityTokenHandler();
            return handler.CanReadToken(tokenStr) ? handler.ReadToken(tokenStr) as JwtSecurityToken : null;
        }
    }
}

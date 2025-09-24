using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Intern.ServiceModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Intern
{
    public class JWTToken
    {
        public JwtSecurityToken GenerateJWTToken(IConfiguration configuration, UserSM user)
        {
            DateTime expiryDate;
            Console.WriteLine("JWT Secret: " + configuration["JWT:Secret"]);


            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            Console.WriteLine("Claims :" + claims);

            claims.Add(new Claim("UserId", user.Id.ToString()));
            claims.Add(new Claim("Email", user.Email));
            // claims.Add(new Claim("Role", user.Role.ToString())); // enum → string
            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            claims.Add(new Claim("LoginId", user.LoginId.ToString()));



            var token = new JwtSecurityToken(
               issuer: configuration["JWT:ValidIssuer"],
               audience: configuration["JWT:ValidAudience"],
               expires: DateTime.Now.AddDays(50),
               claims: claims,
               signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)

            );


            return token;
        }
    }
}

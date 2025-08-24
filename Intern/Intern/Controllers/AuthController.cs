using System.Net;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Authservices _authservices;

        public AuthController(Authservices authservices)
        {
            _authservices = authservices;
        }
        [HttpPost("signup")]
        public async Task<ApiResponse<string>> SignUp(SignUpSM signUpSM)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                // Combine all error messages into one string
                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
               
            }
            
                var resultMessage = await _authservices.SignUpAsync(signUpSM);
            return ApiResponse<string>.SuccessResponse(null,resultMessage);


        }
        [HttpPost("verifyemail")]
        public async Task<ApiResponse<string>> VerifyEmail([FromBody] VerifyEmailSM verifyEmailSM)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                // Combine all error messages into one string
                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);

            }

                var message = await _authservices.VerifyEmailAsync(verifyEmailSM.Token);

                return ApiResponse<string>.SuccessResponse(null, message);

            
        }




        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseSM>> Login([FromBody] LoginSM loginSM)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
            }

            var result = await _authservices.LoginAsync(loginSM);

            return ApiResponse<LoginResponseSM>.SuccessResponse(
                result,
                "Login successfully"
            );
        }

        [HttpPost("googlesignup")]
        [HttpPost("googlelogin")]
        public async Task<ApiResponse<LoginResponseSM>> GoogleAuth([FromBody] GoogleSM googleSM)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
            }

          
            var response = await _authservices.ProcessGoogleIdTokenAsync(googleSM);

            string message = googleSM.IsLogin
                ? "Login successful"
                : "User registered successfully via Google";

            return ApiResponse<LoginResponseSM>.SuccessResponse(response, message);
        }

        



        [Authorize(Roles = "ClientEmployee")]
        [HttpPost("ChangePassword")]

        public async Task<ApiResponse<string>> ChangePassword([FromBody] ChangePasswordSM changePasswordSM)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
            }

            var message = await _authservices.ChangePassword(changePasswordSM);

            return ApiResponse<string>.SuccessResponse(message);
        }
        [HttpPost("forgotpassword")]
        public async Task<ApiResponse<string>> ForgotPassword( string email)
        {
            if (string.IsNullOrEmpty(email) || email.Trim().ToLower() == "null")
            {
                throw new AppException("Email cannot be null or empty.", HttpStatusCode.BadRequest);
            }

           
                var message = await _authservices.ForgotPasswordAsync(email);
            return ApiResponse<string>.SuccessResponse(null,message);

                
        }

        [HttpPost("resetpassword")]
        public async Task<ApiResponse<string>> ResetPassword([FromBody] ResetPasswordSM resetPasswordSM)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                throw new AppException(string.Join(" | ", errors), HttpStatusCode.BadRequest);
            }


            
                var message = await _authservices.ResetPasswordAsync(resetPasswordSM);

               return ApiResponse<string>.SuccessResponse(null,message);

           
        }



    }

}











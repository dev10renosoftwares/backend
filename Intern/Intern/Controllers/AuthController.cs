using Common.Helpers;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Intern.Services;
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
            try
            {
                // Call service
                var resultMessage = await _authservices.SignUpAsync(signUpSM);

                // Return directly as success
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = resultMessage,
                    Data = null
                };
            }
            catch (AppException ex)
            {
                // Return custom exception as response
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // Return any unexpected errors
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
        [HttpPost("verifyemail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailSM verifyEmailSM)
        {
            var message = await _authservices.VerifyEmailAsync(verifyEmailSM.Token);

            var response = new ApiResponse<string>
            {
                Success = true,
                Message = message
            };

            return Ok(response);
        }


       
      
        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseSM>> Login([FromBody] LoginSM loginSM)
        
        {
            try
            {
        
                var result = await _authservices.LoginAsync(loginSM);

                return new ApiResponse<LoginResponseSM>
                {
                    Success = true,
                    Message = "Login successfully",
                    Data = result
                };
            }
            catch (AppException ex)
            {
                return new ApiResponse<LoginResponseSM>
                {
                    Success = false,
                    Message = "Login failed",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseSM>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        [HttpPost("googlesignup")]
        [HttpPost("googlelogin")]
        public async Task<ApiResponse<LoginResponseSM>> GoogleLogin([FromBody] GoogleSM googleSM)
        {
            try
            {
                // Call the service and get both the response and the isNewUser flag
                var (response, isNewUser) = await _authservices.ProcessGoogleIdTokenAsync(googleSM);

                if (isNewUser)
                {
                    // Only show message for new user
                    return new ApiResponse<LoginResponseSM>
                    {
                        Success = true,
                        Message = "User registered successfully via Google",
                        Data = null
                    };
                }
                else
                {
                    // Existing user → return data and message
                    return new ApiResponse<LoginResponseSM>
                    {
                        Success = true,
                        Message = "Login successful",
                        Data = response
                    };
                }
            }
            catch (AppException ex)
            {
                return new ApiResponse<LoginResponseSM>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseSM>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }


    }

}











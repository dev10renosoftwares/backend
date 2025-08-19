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



    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Common.Helpers;
using Google.Apis.Auth;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Intern.Services
{
    public class Authservices
    {
        private readonly ApiDbContext _Context;
        private readonly IMapper _mapper;
        private readonly PasswordHelper _passwordHelper;
        private readonly ImageHelper _imageHelper;
        private readonly EmailService _emailService;
        private readonly EncryptionHelper _encryptionHelper;
        private readonly IConfiguration _configuration;
        private readonly TokenHelper _tokenhelper;
        JWTToken JWTToken = new JWTToken();

        public Authservices(ApiDbContext dbContext,IMapper mapper,PasswordHelper passwordHelper,ImageHelper imageHelper,EmailService emailService,EncryptionHelper encryptionHelper,IConfiguration configuration,TokenHelper tokenHelper )
        {
            _Context = dbContext;
            _mapper = mapper;
            _passwordHelper = passwordHelper;
            _imageHelper = imageHelper;
            _emailService = emailService;
            _encryptionHelper = encryptionHelper;
            _configuration = configuration;
            _tokenhelper = tokenHelper;
        }

       
        public async Task<string> SignUpAsync(SignUpSM signUpSM)
        {
          
            var existingUser = await _Context.ClientUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == signUpSM.Email.ToLower());

            if (existingUser != null)
                throw new AppException("Email already exists", HttpStatusCode.Conflict);

          
            var user = _mapper.Map<ClientUserDM>(signUpSM);
            user.Password = _passwordHelper.HashPassword(signUpSM.Password);
            user.LoginId = signUpSM.Email;
            user.IsActive = true;
            user.IsEmailConfirmed = false;
            user.IsMobileNumberConfirmed = false;
            user.Role = DataModels.Enums.UserRoleDM.ClientEmployee;
            user.CreatedOnUtc = DateTime.UtcNow;

            // Handle profile image if provided
            if (!string.IsNullOrEmpty(signUpSM.ImagePath))
            {
                try
                {
                    var directory = Directory.GetCurrentDirectory();
                    string path = Path.Combine(directory, @"Images");
                    string imagePath = await _imageHelper.SaveBase64ImageAsync(signUpSM.ImagePath, path);
                    user.ImagePath = imagePath;
                }
                catch (Exception)
                {
                    throw new AppException("ImagePath must be Base64 of image", HttpStatusCode.BadRequest);
                }
            }

            _Context.ClientUsers.Add(user);
            await _Context.SaveChangesAsync();

            // 3️⃣ Build email verification payload
            var emailVerification = new EmailVerificationSM
            {
                Email = user.Email,  
                ExpiresAt = DateTime.UtcNow.AddHours(1) 
            };

       
             var encryptedToken = _encryptionHelper.Encrypt(emailVerification);

            // Build link
            var verifyLink = $"https://yourdomain.com/verify-email?token={encryptedToken}";

            // Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Verify your email",
                $"Click here to verify your account: {verifyLink}"
            );

            return "Signup successful! Please check your email to verify your account.";
        }

        public async Task<string> VerifyEmailAsync(string token)
        {
            try
            {
               
                var payload = _encryptionHelper.Decrypt<dynamic>(token);

                string tokenEmail = payload.GetProperty("Email").GetString();
                DateTime expiresAt = payload.GetProperty("ExpiresAt").GetDateTime();

                if (DateTime.UtcNow > expiresAt)
                    throw new AppException("Token expired", HttpStatusCode.Conflict);

               
                var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == tokenEmail);
                if (user == null)
                    throw new AppException("User Not Found", HttpStatusCode.NotFound);

                // 4. Mark as verified
                user.IsEmailConfirmed = true;
                _Context.ClientUsers.Update(user);
                await _Context.SaveChangesAsync();

               
                return "Email verified successfully!";
            }
            catch (Exception)
            {
                
                throw;
            }

            
        }

        public async Task<LoginResponseSM> LoginAsync(LoginSM loginSM)
        {
            // 1. Validate role
            if (!Enum.IsDefined(typeof(UserRoleDM), loginSM.Role))
                throw new AppException("Invalid role specified", HttpStatusCode.BadRequest);

            object user = loginSM.Role switch
            {
                UserRoleDM.SuperAdmin or UserRoleDM.SystemAdmin =>
                    await _Context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == loginSM.Email),

                UserRoleDM.ClientEmployee =>
                    await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == loginSM.Email),

                _ => throw new AppException("Unsupported role for login", HttpStatusCode.BadRequest)
            };

            if (user == null)
                throw new AppException("User not found. Invalid Email", HttpStatusCode.NotFound);

            if (user is ClientUserDM clientUser && !clientUser.IsEmailConfirmed)
                throw new AppException("Please verify your email before logging in.", HttpStatusCode.Forbidden);


            // 2. Map to UserSM for password + image handling
            var userSM = _mapper.Map<UserSM>(user);

            // Password checks
            if (string.IsNullOrEmpty(userSM.Password))
                throw new AppException("This account is registered via Google. Please log in using Google login.", HttpStatusCode.BadRequest);

            var isValidPassword = _passwordHelper.VerifyPassword(loginSM.Password, userSM.Password);
            if (!isValidPassword)
                throw new AppException("Incorrect Password", HttpStatusCode.Unauthorized);

            // 3. Handle image BEFORE mapping to response
            if (!string.IsNullOrEmpty(userSM.ImagePath) && File.Exists(userSM.ImagePath))
                userSM.ImagePath = _imageHelper.ConvertFileToBase64(userSM.ImagePath);
            else
                userSM.ImagePath = null;

            // 4. Generate JWT
            userSM.Role = loginSM.Role; 
            var token = JWTToken.GenerateJWTToken(_configuration, userSM);

            // 5. Map to LoginResponseSM
            var responseSM = _mapper.Map<LoginResponseSM>(userSM);
            responseSM.Token = new JwtSecurityTokenHandler().WriteToken(token);
            responseSM.Expiration = token.ValidTo;

            return responseSM;
        }

        public async Task<LoginResponseSM> ProcessGoogleIdTokenAsync(GoogleSM googleSM)
        {
            try
            {
                // 1. Validate token from Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleSM.IdToken);
                var email = payload.Email;

                if (string.IsNullOrEmpty(email))
                    throw new AppException("Google did not return an email", HttpStatusCode.Conflict);

               
                var clientUser = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == email);

                if (clientUser == null)
                {
                    // New user → create
                    clientUser = new ClientUserDM
                    {
                        Email = email,
                        LoginId = email,
                        Role = UserRoleDM.ClientEmployee,
                        IsEmailConfirmed = true,
                        Password = null,
                        CreatedOnUtc = DateTime.UtcNow,
                        IsActive = true
                    };

                    if (!string.IsNullOrEmpty(payload.Picture))
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), @"Images");
                        clientUser.ImagePath = await _imageHelper.SaveImageFromUrlAsync(payload.Picture, path);
                    }

                    try
                    {
                        _Context.ClientUsers.Add(clientUser);
                        await _Context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw new AppException("Database error while saving client user", HttpStatusCode.Conflict);
                    }
                }
                else
                {
                    // Existing user → confirm email if not confirmed
                    if (!clientUser.IsEmailConfirmed)
                    {
                        clientUser.IsEmailConfirmed = true;
                        _Context.ClientUsers.Update(clientUser);
                        await _Context.SaveChangesAsync();
                    }
                }

                // 3. Ensure exists in ExternalUsers
                var externalUser = await _Context.ExternalUsers.FirstOrDefaultAsync(eu => eu.UserId == clientUser.Id);

                if (externalUser == null)
                {
                    externalUser = new ExternalUserDM
                    {
                        UserId = clientUser.Id,
                        RefreshToken = googleSM.RefreshToken,
                        LoginType = LoginTypeDM.Google,
                        CreatedOnUtc = DateTime.UtcNow,
                    };

                    try
                    {
                        _Context.ExternalUsers.Add(externalUser);
                        await _Context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw new AppException("Database error while saving externaluser", HttpStatusCode.Conflict);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(googleSM.RefreshToken))
                    {
                        externalUser.RefreshToken = googleSM.RefreshToken;
                        externalUser.LastModifiedOnUtc = DateTime.UtcNow;
                    }

                    try
                    {
                        _Context.ExternalUsers.Update(externalUser);
                        await _Context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw new AppException("Database error while updating externaluser", HttpStatusCode.Conflict);
                    }
                }

                // 4. Map ClientUserDM → UserSM for JWT
                var userSM = _mapper.Map<UserSM>(clientUser);
                var token = JWTToken.GenerateJWTToken(_configuration, userSM);

                // 5. Map ClientUserDM → LoginResponseSM
                var response = _mapper.Map<LoginResponseSM>(clientUser);
                response.Token = new JwtSecurityTokenHandler().WriteToken(token);
                response.Expiration = token.ValidTo;

                if (!string.IsNullOrEmpty(clientUser.ImagePath) && File.Exists(clientUser.ImagePath))
                {
                    response.ImagePath = _imageHelper.ConvertFileToBase64(clientUser.ImagePath);
                }
                else if (!string.IsNullOrEmpty(payload.Picture))
                {
                    response.ImagePath = await _imageHelper.ConvertImageUrlToBase64Async(payload.Picture);

                    string path = Path.Combine(Directory.GetCurrentDirectory(), @"Images");
                    clientUser.ImagePath = await _imageHelper.SaveImageFromUrlAsync(payload.Picture, path);
                    await _Context.SaveChangesAsync();
                }

                return response; 
            }
            catch (Exception ex)
            {
                throw new AppException($"Error: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<string> ChangePassword(ChangePasswordSM changePasswordSM)
        {
            if (changePasswordSM.NewPassword != changePasswordSM.ConfirmPassword)
            {
                throw new AppException("New password and confirm password do not match", HttpStatusCode.Conflict);
            }

            var oldPasswordHash = _passwordHelper.HashPassword(changePasswordSM.OldPassword);
            var newPasswordHash = _passwordHelper.HashPassword(changePasswordSM.NewPassword);


            var userId = _tokenhelper.GetUserIdFromToken();

            var user = await _Context.ClientUsers.FirstOrDefaultAsync(cu => cu.Id == userId);

            if (user == null)
            {
                throw new AppException("User not found", HttpStatusCode.NotFound);
            }

            // Validate old password
            if (!_passwordHelper.VerifyPassword(changePasswordSM.OldPassword, user.Password))
            {
                throw new AppException("Incorrect old password", HttpStatusCode.BadRequest);
            }

            // Prevent same password reuse
            if (_passwordHelper.VerifyPassword(changePasswordSM.NewPassword, user.Password))
            {
                throw new AppException("New password must be different from old password", HttpStatusCode.BadRequest);
            }

           
            user.Password = _passwordHelper.HashPassword(changePasswordSM.NewPassword);

            await _Context.SaveChangesAsync();

            return "Password changed successfully";
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found");

            // payload for token
            var payload = new ResetPasswordPayloadSM
            {
                Email = user.Email,
                RequestedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            var authCode = _encryptionHelper.Encrypt(payload);

            var frontendUrl = "https://myblogapp.com/reset-password";
            var resetLink = $"{frontendUrl}?authcode={Uri.EscapeDataString(authCode)}";

            await _emailService.SendEmailAsync(user.Email, "Password Reset",
                $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>");

            return "Reset link sent to your email";
    
        }

        

        public async Task<string> ResetPasswordAsync(ResetPasswordSM resetPasswordSM)
        {
            if (resetPasswordSM == null)
                throw new ArgumentNullException(nameof(resetPasswordSM));

            ResetPasswordPayloadSM payload;
            try
            {
                payload = _encryptionHelper.Decrypt<ResetPasswordPayloadSM>(resetPasswordSM.AuthCode);
            }
            catch
            {
                throw new Exception("Invalid or tampered link");
            }

            if (payload.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Reset link expired");

            var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null)
                throw new Exception("User not found");

            user.Password = _passwordHelper.HashPassword(resetPasswordSM.NewPassword);
            _Context.ClientUsers.Update(user);
            await _Context.SaveChangesAsync();

            return "Password reset successful";
        }
    }
 }









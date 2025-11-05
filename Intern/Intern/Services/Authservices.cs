using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Common.Helpers;
using Google.Apis.Auth;
using Intern.Common;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.Enums;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Intern.ServiceModels.Enums;
using Intern.ServiceModels.User;

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
        private readonly AuthSettings _authSettings;
        
        JWTToken JWTToken = new JWTToken();

        public Authservices(ApiDbContext dbContext,IMapper mapper,PasswordHelper passwordHelper,ImageHelper imageHelper,EmailService emailService,EncryptionHelper encryptionHelper,IConfiguration configuration,TokenHelper tokenHelper, IOptions<AuthSettings> authSettings )
        {
            _Context = dbContext;
            _mapper = mapper;
            _passwordHelper = passwordHelper;
            _imageHelper = imageHelper;
            _emailService = emailService;
            _encryptionHelper = encryptionHelper;
            _configuration = configuration;
            _tokenhelper = tokenHelper;
            _authSettings = authSettings.Value;
        }

        public async Task<bool> VerifyEmail(EmailExistsSM emailExists)
        {
            var email = emailExists.Email.ToLower();

            var clientUserExists = await _Context.ClientUsers
                .AnyAsync(u => u.Email.ToLower() == email);

            if (clientUserExists)
                return true;

            var appUserExists = await _Context.ApplicationUsers
                .AnyAsync(u => u.Email.ToLower() == email);

            if (appUserExists)
                return true;

            return false;
        }



        public async Task<string> SignUpAsync(SignUpSM signUpSM)
        {
          
            var existingUser = await _Context.ClientUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == signUpSM.Email.ToLower());

            if (existingUser != null)
                throw new AppException("Email already exists", HttpStatusCode.Forbidden);

          
            var user = _mapper.Map<ClientUserDM>(signUpSM);
            user.Password = _passwordHelper.HashPassword(signUpSM.Password);
            int atIndex = signUpSM.Email.IndexOf('@');
            user.LoginId = atIndex > 0
                ? signUpSM.Email.Substring(0, atIndex).ToLower()
                : signUpSM.Email.ToLower();
            //user.LoginId = signUpSM.Email;
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
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.EmailVerificationExpiryMinutes)
            };

       
             var encryptedToken = _encryptionHelper.Encrypt(emailVerification);

            // Build link
             var verifyLink = $"http://localhost:4200/verify-email?token={encryptedToken}";

            #region EmailBody
            var emailBody = $@"
            <html>
            <head>
              <style>
                .button {{
                  background-color: #4CAF50;
                  border: none;
                  color: white;
                  padding: 12px 24px;
                  text-align: center;
                  text-decoration: none;
                  display: inline-block;
                  font-size: 16px;
                  margin: 16px 0;
                  cursor: pointer;
                  border-radius: 6px;
                }}
                .content {{
                  font-family: Arial, sans-serif;
                  line-height: 1.6;
                  color: #333333;
                }}
              </style>
            </head>
            <body>
              <div class='content'>
                <h2>Hello {user.Email},</h2>
                <p>Thank you for registering with us. Please verify your email address to complete your account setup.</p>
                <p style='text-align:center;'>
                  <a href='{verifyLink}' class='button'>Verify Email</a>
                </p>
                <p>If the button above does not work, copy and paste the following link into your browser:</p>
                <p><a href='{verifyLink}'>{verifyLink}</a></p>
                <p>Best regards,<br/>Your Company Name</p>
              </div>
            </body>
            </html>
            ";

            #endregion EmailBody

            // Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Verify your email",
                emailBody 
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

                if (user.IsEmailConfirmed)
                    throw new AppException("Email is already verified.", HttpStatusCode.Conflict);


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
        public async Task<string> ResendVerificationEmailAsync(string email)
        {
            var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new AppException("User not found", HttpStatusCode.NotFound);

            if (user.IsEmailConfirmed)
                throw new AppException("Email already verified", HttpStatusCode.Conflict);

            // Build a fresh verification payload
            var emailVerification = new EmailVerificationSM
            {
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.EmailVerificationExpiryMinutes)
            };

            var encryptedToken = _encryptionHelper.Encrypt(emailVerification);

            var verifyLink = $"http://localhost:4200/verify-email?token={encryptedToken}";

            #region EmailBody
            var emailBody = $@"
            <html>
            <head>
              <style>
                .button {{
                  background-color: #4CAF50;
                  border: none;
                  color: white;
                  padding: 12px 24px;
                  text-align: center;
                  text-decoration: none;
                  display: inline-block;
                  font-size: 16px;
                  margin: 16px 0;
                  cursor: pointer;
                  border-radius: 6px;
                }}
                .content {{
                  font-family: Arial, sans-serif;
                  line-height: 1.6;
                  color: #333333;
                }}
              </style>
            </head>
            <body>
              <div class='content'>
                <h2>Hello {user.Email},</h2>
                <p>Thank you for registering with us. Please verify your email address to complete your account setup.</p>
                <p style='text-align:center;'>
                  <a href='{verifyLink}' class='button'>Verify Email</a>
                </p>
                <p>If the button above does not work, copy and paste the following link into your browser:</p>
                <p><a href='{verifyLink}'>{verifyLink}</a></p>
                <p>Best regards,<br/>Your Company Name</p>
              </div>
            </body>
            </html>
            ";

            #endregion EmailBody

            await _emailService.SendEmailAsync(user.Email, "Resend Email Verification", emailBody);

            return "Verification email resent. Please check your inbox.";
        }



        /*public async Task<LoginResponseSM> LoginAsync(LoginSM loginSM)
        {
            // 1. Validate role
            if (!Enum.IsDefined(typeof(UserRoleSM), loginSM.Role))
                throw new AppException("Invalid role specified", HttpStatusCode.BadRequest);

            object user = loginSM.Role switch
            {
                UserRoleSM.SuperAdmin or UserRoleSM.SystemAdmin =>
                    await _Context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == loginSM.Email),

                UserRoleSM.ClientEmployee =>
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
            if (string.IsNullOrEmpty(user.))
                throw new AppException("This account is registered via Google. Please log in using Google login.", HttpStatusCode.BadRequest);

            var isValidPassword = _passwordHelper.VerifyPassword(loginSM.Password, userSM.Password);
            if (!isValidPassword)
                throw new AppException("Incorrect Password", HttpStatusCode.Unauthorized);

            // 3. Handle image BEFORE mapping to response
            if (!string.IsNullOrEmpty(userSM.ImageBase64) && File.Exists(userSM.ImageBase64))
                userSM.ImageBase64 = _imageHelper.ConvertFileToBase64(userSM.ImageBase64);
            else
                userSM.ImageBase64 = null;

            // 4. Generate JWT
            userSM.Role = loginSM.Role; 
            var token = JWTToken.GenerateJWTToken(_configuration, userSM);

            // 5. Map to LoginResponseSM
            var responseSM = _mapper.Map<LoginResponseSM>(userSM);
            responseSM.Token = new JwtSecurityTokenHandler().WriteToken(token);
            responseSM.Expiration = token.ValidTo;

            return responseSM;
        }*/

        public async Task<LoginResponseSM> LoginAsync(LoginSM loginSM)
        {
            if (loginSM == null) throw new ArgumentNullException(nameof(loginSM));

            object? dbUser = null;
            string? imagePath = null;
            bool isPasswordPresent = false;

            // 1. Fetch user from DB based on role
            switch (loginSM.Role)
            {
                case UserRoleSM.SuperAdmin:
                case UserRoleSM.SystemAdmin:
                    var appUser = await _Context.ApplicationUsers
                        .FirstOrDefaultAsync(x => x.LoginId == loginSM.Email || x.Email == loginSM.Email);

                    if (appUser == null)
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    // Check password existence
                    isPasswordPresent = !string.IsNullOrEmpty(appUser.Password);

                    if (string.IsNullOrEmpty(appUser.Password))
                        throw new AppException("Social login used. Set a profile password for custom login", HttpStatusCode.BadRequest);

                    if (!_passwordHelper.VerifyPassword(loginSM.Password, appUser.Password))
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    if (appUser.Role != (UserRoleDM)loginSM.Role)
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    dbUser = appUser;
                    imagePath = appUser.ImagePath;
                    break;

                case UserRoleSM.ClientEmployee:
                    var clientUser = await _Context.ClientUsers
                        .FirstOrDefaultAsync(x => x.LoginId == loginSM.Email || x.Email == loginSM.Email);

                    if(clientUser.IsEmailConfirmed == false)
                        throw new AppException("Please verify your email address before logging in.", HttpStatusCode.Conflict);

                    // Check password existence
                    isPasswordPresent = !string.IsNullOrEmpty(clientUser.Password);

                    if (clientUser == null)
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    if (string.IsNullOrEmpty(clientUser.Password))
                        throw new AppException("Social login used. Set a profile password for custom login", HttpStatusCode.BadRequest);

                    if (!_passwordHelper.VerifyPassword(loginSM.Password, clientUser.Password))
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    if (clientUser.Role != (UserRoleDM)loginSM.Role)
                        throw new AppException("User Not Found", HttpStatusCode.Unauthorized);

                    dbUser = clientUser;
                    imagePath = clientUser.ImagePath;
                    break;

                default:
                    throw new AppException("Invalid Role", HttpStatusCode.BadRequest);
            }

            // 2. Map DB user to UserSM
            UserSM userSM = dbUser switch
            {
                ApplicationUserDM app => _mapper.Map<UserSM>(app),
                ClientUserDM client => _mapper.Map<UserSM>(client),
                _ => throw new AppException("Invalid user type", HttpStatusCode.BadRequest)
            };

            // 3. Handle image conversion
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                userSM.ImageBase64 = _imageHelper.ConvertFileToBase64(imagePath);
            else
                userSM.ImageBase64 = null;

            // 4. Generate JWT
            var token = JWTToken.GenerateJWTToken(_configuration, userSM);

            // 5. Map to LoginResponseSM
            var responseSM = _mapper.Map<LoginResponseSM>(userSM);
            responseSM.ImagePath = userSM.ImageBase64;
            responseSM.Token = new JwtSecurityTokenHandler().WriteToken(token);
            responseSM.Expiration = token.ValidTo;
            responseSM.IsPasswordPresent = isPasswordPresent;

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
                        Name = payload.Name,
                        // LoginId = email,
                        LoginId = !string.IsNullOrEmpty(email) && email.Contains("@")
                              ? email[..email.IndexOf('@')].ToLower()
                              : email.ToLower(),
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

                // ✅ 6. Set IsPasswordPresent based on DB record
                response.IsPasswordPresent = !string.IsNullOrEmpty(clientUser.Password);

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

        public async Task<string> ChangePassword(int id, ChangePasswordSM changePasswordSM)
        {
            if (changePasswordSM.NewPassword != changePasswordSM.ConfirmPassword)
            {
                throw new AppException("New password and confirm password do not match", HttpStatusCode.Conflict);
            }

            var oldPasswordHash = _passwordHelper.HashPassword(changePasswordSM.OldPassword);
            var newPasswordHash = _passwordHelper.HashPassword(changePasswordSM.NewPassword);

            var user = await _Context.ClientUsers.FirstOrDefaultAsync(cu => cu.Id == id);

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
                throw new AppException("User not found",HttpStatusCode.NotFound);

            // payload for token
            var payload = new ResetPasswordPayloadSM
            {
                Email = user.Email,
                RequestedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.ResetPasswordExpiryMinutes)
            };

            var authCode = _encryptionHelper.Encrypt(payload);

            var frontendUrl = "http://localhost:4200/reset-password";
            var resetLink = $"{frontendUrl}?authcode={Uri.EscapeDataString(authCode)}";


            #region EmailBody
            var emailBody = $@"
    <html>
    <head>
      <style>
        .button {{
          background-color: #007BFF;
          border: none;
          color: white;
          padding: 12px 24px;
          text-align: center;
          text-decoration: none;
          display: inline-block;
          font-size: 16px;
          margin: 16px 0;
          cursor: pointer;
          border-radius: 6px;
        }}
        .content {{
          font-family: Arial, sans-serif;
          line-height: 1.6;
          color: #333333;
        }}
      </style>
    </head>
    <body>
      <div class='content'>
        <h2>Hello {user.Email},</h2>
        <p>We received a request to reset your password. Click the button below to reset it:</p>
        <p style='text-align:center;'>
          <a href='{resetLink}' class='button'>Reset Password</a>
        </p>
        <p>If the button above does not work, copy and paste this link into your browser:</p>
        <p><a href='{resetLink}'>{resetLink}</a></p>
        <p>This link will expire in 30 minutes.</p>
        <p>If you did not request a password reset, please ignore this email.</p>
        <p>Best regards,<br/>Your Company Name</p>
      </div>
    </body>
    </html>
    ";

            #endregion EmailBody



            await _emailService.SendEmailAsync(user.Email, "Password Reset", emailBody);




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
                throw new AppException("Invalid or tampered link",HttpStatusCode.BadRequest);
            }

            if (payload.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Reset link expired",HttpStatusCode.Gone);

            var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null)
                throw new AppException("User not found",HttpStatusCode.NotFound);

            user.Password = _passwordHelper.HashPassword(resetPasswordSM.NewPassword);
            _Context.ClientUsers.Update(user);
            await _Context.SaveChangesAsync();

            return "Password reset successful";
        }

        public async Task<string> SetPasswordAsync(int userId, SetPasswordSM model)
        {

            if (model.Password != model.ConfirmPassword)
                throw new AppException("Password and Confirm Password do not match.", HttpStatusCode.BadRequest);

            var user = await _Context.ClientUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new AppException("User not found.", HttpStatusCode.NotFound);

            var loginid = _tokenhelper.GetLoginIdFromToken();

            user.Password = _passwordHelper.HashPassword(model.Password);
            user.LastModifiedOnUtc = DateTime.UtcNow;
            user.LastModifiedBy = loginid;

            _Context.ClientUsers.Update(user);
            await _Context.SaveChangesAsync();

            return "Password has been set successfully.";
        }

    }
}









using System.Net;
using AutoMapper;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.BaseServiceModels;
using Microsoft.EntityFrameworkCore;

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

        public Authservices(ApiDbContext dbContext,IMapper mapper,PasswordHelper passwordHelper,ImageHelper imageHelper,EmailService emailService,EncryptionHelper encryptionHelper)
        {
            _Context = dbContext;
            _mapper = mapper;
            _passwordHelper = passwordHelper;
            _imageHelper = imageHelper;
            _emailService = emailService;
            _encryptionHelper = encryptionHelper;
        }

       
        public async Task<string> SignUpAsync(SignUpSM signUpSM)
        {
            // 1️⃣ Check if email exists
            var existingUser = await _Context.ClientUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == signUpSM.Email.ToLower());

            if (existingUser != null)
                throw new AppException("Email already exists", HttpStatusCode.Conflict);

            // 2️⃣ Map DTO to entity
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
                    throw new AppException("Image save failed", HttpStatusCode.Conflict);
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

            // Encrypt the payload
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

                // 3. Find user
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


    }
}

using AutoMapper;
using Common.Helpers;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.User;
using System.Net;

namespace Intern.Services
{
    public class UserService
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ImageHelper _imageHelper;

        public UserService(ApiDbContext context, IMapper mapper,ImageHelper imageHelper)
        {
            _context = context;
            _mapper = mapper;
            _imageHelper = imageHelper;
        }

        public async Task<ClientUserSM> GetByIdAsync(int id)
        {
            // First check ClientUsers
            var clientUser = await _context.ClientUsers.FindAsync(id);
            if (clientUser != null)
            {

                var sm = _mapper.Map<ClientUserSM>(clientUser);

                if (!string.IsNullOrEmpty(clientUser.ImagePath))
                {
                    // convert image path to base64
                    sm.ImageBase64 = _imageHelper.ConvertFileToBase64(clientUser.ImagePath) ;
                }
                sm.UserName = clientUser.LoginId;
                sm.Password = null;
                return sm;
            }
            // If neither found
            throw new AppException("User not found", HttpStatusCode.NotFound);
        }

        public async Task<ClientUserSM> UpdateAsync(int id, ClientUserSM objSM)
        {
            var userDM = await _context.ClientUsers.FindAsync(id);
            if (userDM == null)
            {
                throw new AppException("User not found", HttpStatusCode.NotFound);
            }

            // ✅ Map incoming values into the tracked entity
            _mapper.Map(objSM, userDM);

            // ✅ Keep values that should not be overridden
            userDM.Id = userDM.Id; // EF handles this, but for safety
            userDM.Role = userDM.Role;
            userDM.Password = userDM.Password;
            userDM.Email = userDM.Email;
            userDM.IsEmailConfirmed = userDM.IsEmailConfirmed;
            userDM.IsActive = userDM.IsActive;
            userDM.IsMobileNumberConfirmed = userDM.IsMobileNumberConfirmed;

            userDM.LastModifiedBy = "null user";
            userDM.LastModifiedOnUtc = DateTime.UtcNow;

            // ✅ Handle image
            if (!string.IsNullOrEmpty(objSM.ImageBase64))
            {
                var directory = Directory.GetCurrentDirectory();
                var newPath = Path.Combine(directory, @"Images");

                // delete old image if it exists
                if (!string.IsNullOrEmpty(userDM.ImagePath) && File.Exists(userDM.ImagePath))
                {
                    File.Delete(userDM.ImagePath);
                }

                // save new image
                userDM.ImagePath = await _imageHelper.SaveBase64ImageAsync(objSM.ImageBase64, newPath);
            }

            
            try {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            // ✅ Prepare response
            var userSM = _mapper.Map<ClientUserSM>(userDM);

            // Convert existing file path to Base64 if file exists
            if (!string.IsNullOrEmpty(userDM.ImagePath) && File.Exists(userDM.ImagePath))
            {
                userSM.ImageBase64 = Convert.ToBase64String(File.ReadAllBytes(userDM.ImagePath));
            }

            return userSM;

        }



    }
}

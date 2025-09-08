using AutoMapper;
using Intern.DataModels.Exams;
using Intern.DataModels.User;
using Intern.ServiceModels;

namespace Intern.Common.Mapper
{
    public class MappingProfile
    {

            public class UserProfile : Profile
            {
                public UserProfile()
                {
                    // Signup mapping
                    CreateMap<SignUpSM, ClientUserDM>();

                    // Domain -> Service mappings
                    CreateMap<ApplicationUserDM, UserSM>().ReverseMap();
                    CreateMap<ClientUserDM, UserSM>().ReverseMap();
                   

                    // UserSM -> LoginResponseSM
                    CreateMap<UserSM, LoginResponseSM>();

        

                    // ClientUserDM -> LoginResponseSM (for direct mapping)
                  CreateMap<ClientUserDM, LoginResponseSM>()
                    .ForMember(dest => dest.Token, opt => opt.Ignore())
                    .ForMember(dest => dest.Expiration, opt => opt.Ignore())
                    .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

                     //Department
                     CreateMap<AddDepartmentSM, DepartmentDM>()
                     .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore());

                // **Add this mapping for your GetAllAsync / GetByIdAsync**
                CreateMap<DepartmentDM, DepartmentSM>().ReverseMap();


                // ✅ Post mappings
                CreateMap<PostDM, PostSM>().ReverseMap();
                CreateMap<AddPostSM, PostDM>();




            }
            
            }
     
    }
 }




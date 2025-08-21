using AutoMapper;
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
            }
            }
    }
 }




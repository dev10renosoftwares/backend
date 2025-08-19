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
                CreateMap<SignUpSM, ClientUserDM>();
            }
        }
    }
}

using AutoMapper;
using Intern.DataModels.Enums;
using Intern.DataModels.Exams;
using Intern.DataModels.User;
using Intern.ServiceModels;
using Intern.ServiceModels.Enums;
using Intern.ServiceModels.Exams;
using Intern.ServiceModels.User;

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
                     CreateMap<DepartmentPostsDM, DepartmentPostsSM>().ReverseMap();


                    // UserSM -> LoginResponseSM
                     CreateMap<UserSM, LoginResponseSM>();



                     // ClientUserDM -> LoginResponseSM (for direct mapping)
                   CreateMap<ClientUserDM, LoginResponseSM>()
                     .ForMember(dest => dest.Token, opt => opt.Ignore())
                     .ForMember(dest => dest.Expiration, opt => opt.Ignore())
                     .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

                     //Department
                   CreateMap<DepartmentSM, DepartmentDM>()
                     .ForMember(dest => dest.Id, opt => opt.Ignore())
                     .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore());

                     // **Add this mapping for your GetAllAsync / GetByIdAsync**
                     CreateMap<DepartmentDM, DepartmentSM>().ReverseMap();


                     // ✅ Post mappings
                    CreateMap<PostDM, PostSM>().ReverseMap();

                   CreateMap<PostSM, PostDM>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())           // EF generates PK
                    .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore()); // will set manually


                   // ✅ Map AddPostandAssignSM → PostDM
                   CreateMap<AddPostandAssignSM, PostDM>();


                   // ✅ Map AddPostandAssignSM → DepartmentPostsDM
                    CreateMap<AddPostandAssignSM, DepartmentPostsDM>();

                     // Existing mapping (correct)
                     CreateMap<MCQsDM, MCQsSM>().ReverseMap();
                     CreateMap<NotificationsDM, NotificationsSM>().ReverseMap();
                     CreateMap<NotificationTypeDM, NotificationTypeSM>().ReverseMap();

                    // Updated mapping for UpdateAsync ignoring nulls
                    CreateMap<MCQsSM, MCQsDM>() // <-- use MCQsDM, not MCQs
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


                    // MCQ mapping
                    CreateMap<MCQSubjectPostSM, MCQPostSubjectDM>()
                    .ForMember(dest => dest.SubjectId,
                               opt => opt.MapFrom(src => src.SubjectId))
                    .ForMember(dest => dest.PostId,
                               opt => opt.MapFrom(src => src.PostId))
                    .ForMember(dest => dest.MCQType,
                               opt => opt.MapFrom(src => (int)src.McqType));

                        // ClientUserDM -> UserResponseSM (for fetching users)
                    CreateMap<ClientUserDM, ClientUserSM>()
                        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.LoginId))
                        .ForMember(dest => dest.Password, opt => opt.Ignore())       // don’t expose password
                        .ForMember(dest => dest.ImageBase64, opt => opt.Ignore());    // you set this manually

                    CreateMap<ClientUserSM, ClientUserDM>()
                     .ForMember(dest => dest.Id, opt => opt.Ignore())           // prevent PK overwrite
                    .ForMember(dest => dest.Password, opt => opt.Ignore())     // password handled separately
                    .ForMember(dest => dest.Email, opt => opt.Ignore())        // don’t let user change email
                    .ForMember(dest => dest.Role, opt => opt.Ignore())         // role is managed by system
                    .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null));



                   CreateMap<MCQsDM, MCQsSM>();

            }

        }
            

    }
 }




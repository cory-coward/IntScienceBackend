using AutoMapper;
using IntScience.API.Dtos;
using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Identity;

namespace IntScience.API.Maps;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>()
            .ForMember(u => u.UserName, options => options.MapFrom(x => x.Email));
        CreateMap<ApplicationUser, UserProfileResponseDto>();
        CreateMap<IdentityRole, RoleResponseDto>();
    }
}

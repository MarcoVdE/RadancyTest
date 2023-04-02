namespace RadancyBankingSystemTest.Api.Profiles;

using AutoMapper;
using Domain.Dto.Models;
using Domain.Models;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}
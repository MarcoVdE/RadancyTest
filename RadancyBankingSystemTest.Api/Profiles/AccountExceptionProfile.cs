namespace RadancyBankingSystemTest.Api.Profiles;

using AutoMapper;
using Domain.Dto.Exceptions;
using Domain.Models.Exceptions;

public class AccountExceptionProfile : Profile
{
    public AccountExceptionProfile()
    {
        CreateMap<AccountException, AccountExceptionDto>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(source => (int) source.StatusCode))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(source => source.Data))
            .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(source => source.StackTrace))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(source => source.Message))
            .IgnoreAllPropertiesWithAnInaccessibleSetter();
    }
}
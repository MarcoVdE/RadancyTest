namespace RadancyBankingSystemTest.Api.Profiles;

using AutoMapper;
using Domain.Dto.Models;
using Domain.Models;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountDto>();
    }
}
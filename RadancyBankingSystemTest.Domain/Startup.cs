
using System.Diagnostics.CodeAnalysis;

namespace RadancyBankingSystemTest.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Repositories;
using Repositories.Context;
using Services;

[ExcludeFromCodeCoverage(Justification = "Registering services")]
public static class DomainServices
{
    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.RegisterRepositories();
        services.RegisterServices();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IAccountRepository, AccountRepository>();
        
        services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("Users"));
        services.AddSingleton<IUserRepository, UserRepository>();
    }
    
    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAccountService, UserAccountService>();
    }
}
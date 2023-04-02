namespace RadancyBankingSystemTest.Domain.Interfaces;

using Models;
using Services;

public interface IUserAccountService
{
    /// <inheritdoc cref="UserAccountService.CreateUser"/>
    public Task<Guid> CreateUser(string idNumber, string firstName, string lastName);
    /// <inheritdoc cref="UserAccountService.CreateAccount"/>
    Task<Guid> CreateAccount(Guid userId);
    Task<Account> GetUserAccount(Guid userId, Guid accountId);
    /// <inheritdoc cref="UserAccountService.GetUser"/>
    Task<User> GetUser(Guid userId);
    Task DeleteAccount(Guid userId, Guid accountId);
    Task<Account> UpdateAccount(Guid userId, Guid accountId, decimal amount);
}
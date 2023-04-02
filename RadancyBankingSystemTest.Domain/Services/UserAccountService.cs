namespace RadancyBankingSystemTest.Domain.Services;

using Microsoft.Extensions.Options;
using Configuration;
using Interfaces;
using Models;
using Models.Exceptions.Factories;

public class UserAccountService : IUserAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly AccountOptions _accountOptions;

    public UserAccountService(IUserRepository userRepository, IAccountRepository accountRepository,
        IOptions<AccountOptions> accountOptions)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _accountOptions = accountOptions.Value;
    }
    
    /// <inheritdoc cref="IUserRepository.Create"/>
    public async Task<Guid> CreateUser(string idNumber, string firstName, string lastName)
    {
        return await _userRepository.Create(idNumber, firstName, lastName);
    }

    /// <summary>
    /// Create an account for a user.
    /// A user may have as many accounts as they like.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<Guid> CreateAccount(Guid userId)
    {
        return Task.FromResult(_accountRepository.Create(userId, _accountOptions.StartingAccountBalance));
    }

    /// <summary>
    /// Get a user's account
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<Account> GetUserAccount(Guid userId, Guid accountId)
    {
        // Would usually not check if user exists due to in access token
        var account = await _accountRepository.Get(accountId);
        if (account.UserId != userId)
            throw new KeyNotFoundException();

        return account;
    }
    
    /// <inheritdoc cref="IUserRepository.Get"/>
    public async Task<User> GetUser(Guid userId)
    {
        return await _userRepository.Get(userId);
    }

    public async Task DeleteAccount(Guid userId, Guid accountId)
    {
        var account = await _accountRepository.Get(accountId);
        if (account.UserId != userId) throw new KeyNotFoundException();
        
        _accountRepository.Delete(accountId);
    }

    public async Task<Account> UpdateAccount(Guid userId, Guid accountId, decimal amount)
    {
        var account = await GetUserAccount(userId, accountId);
        
        // Business rule: account may not withdraw above 90% of its maximum value in one transaction
        if (amount >= _accountOptions.MaxDepositLimit)
            throw AccountExceptionFactory.MaxDepositLimit(_accountOptions.MaxDepositLimit);
        
        // Business rule: account must stay above min. value
        if (account.Balance + amount < _accountOptions.MinAccountBalance)
            throw AccountExceptionFactory.BalanceTooLow(accountId);

        // Business rule: account may not withdraw above 90% of its maximum value in one transaction
        if (amount < 0 && Math.Abs(amount) > account.Balance * (decimal) 0.9)
            throw AccountExceptionFactory.MaxWithdrawLimit();

        return _accountRepository.Update(accountId, userId, account.Balance, account.Balance + amount);
    }
}
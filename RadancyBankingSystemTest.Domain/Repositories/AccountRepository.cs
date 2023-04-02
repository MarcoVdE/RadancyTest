
namespace RadancyBankingSystemTest.Domain.Repositories;

using System.Collections.Concurrent;
using Interfaces;
using Models;
using Models.Exceptions.Factories;

public class AccountRepository : IAccountRepository
{
    // Showing off a different way doing an "in-memory" DB, could also use MS in-memory cache with time-out,
    //  if real DB, would use that instead for something so frequent accessed and make it store in a distributed cache
    //  (e.g. redis)  for horizontal scaling
    internal readonly ConcurrentDictionary<Guid, Account> Accounts = new();

    public Guid Create(Guid userId, decimal startingBalance)
    {
        while (true)
        {
            // Chance of collision is very low, could handle error rate. Also would be done sql side using auto PK.
            var accountId = Guid.NewGuid();
            if (Accounts.TryAdd(accountId, new Account(userId, accountId, startingBalance)))
            {
                return accountId;
            };
        }
    }
    
    public Task<Account> Get(Guid accountId)
    {
        if (!Accounts.TryGetValue(accountId, out var account)) 
            throw new KeyNotFoundException($"Account not found {accountId}");
        
        // purposefully made async since this would usually be DB/Redis call, as mock.
        return Task.FromResult(account);
    }

    public Account Update(Guid accountId, Guid userId, decimal balanceBefore, decimal balanceAfter)
    {
        lock (Accounts)
        {
            if (!Accounts.TryGetValue(accountId, out var account) || account.UserId != userId) 
                throw new KeyNotFoundException($"Account not found {accountId}");
            
            // Depending on the sensitivity of the resource, could also say key not found for the account instead as
            //  no account for user, would be better for banking app usually, but if e.g. user has read access
            // but no write access, forbidden is the correct answer:
            // if (account.UserId != userId)
            //   throw UserExceptionFactory.ForbiddenException(userId);
            
            // This is to stop race conditions due to business rule checks.
            if (account.Balance != balanceBefore)
                throw AccountExceptionFactory.InvalidAccountState();

            account.Balance = balanceAfter;
            return account;
        }
    }
    
    public void Delete(Guid accountId)
    {
        // Doesn't matter if successful or not, account doesn't exist for user, "fire and forget"
        Accounts.TryRemove(accountId, out _);
    }
}
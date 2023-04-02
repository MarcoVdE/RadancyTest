namespace RadancyBankingSystemTest.Domain.Interfaces;

using Models;

public interface IAccountRepository
{
    /// <summary>
    /// Create an account for the user and return an account Id. 
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="startingBalance"></param>
    /// <returns></returns>
    Guid Create(Guid UserId, decimal startingBalance);

    Task<Account> Get(Guid accountId);
    
    /// <summary>
    /// The before and after is due to race conditions, repo is singleton, if balance before/after is still the same, can update
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="userId"></param>
    /// <param name="balanceBefore"></param>
    /// <param name="balanceAfter"></param>
    Account Update(Guid accountId, Guid userId, decimal balanceBefore, decimal balanceAfter);
    
    void Delete(Guid accountId);
}
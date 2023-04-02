using System.Net;

namespace RadancyBankingSystemTest.Domain.Models.Exceptions.Factories;

public static class AccountExceptionFactory
{
    public static AccountException MaxDepositLimit(decimal limit) =>
        new($"You may not deposit more than {limit} in a single transaction");
    
    public static AccountException BalanceTooLow(Guid accountId) =>
        new($"Your balance for account: {accountId} is too low to withdraw that amount");

    public static AccountException MaxWithdrawLimit() =>
        new("You may not withdraw more than 90% of your account balance in one transaction");
    
    public static AccountException InvalidAccountState() =>
        new($"Your account balance changed during the transaction, please try again", HttpStatusCode.Conflict);
}
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RadancyBankingSystemTest.Domain.Configuration;
using RadancyBankingSystemTest.Domain.Interfaces;
using RadancyBankingSystemTest.Domain.Models;
using RadancyBankingSystemTest.Domain.Models.Exceptions;
using RadancyBankingSystemTest.Domain.Services;

namespace RadancyBankingSystemTest.Domain.Tests.Services;

[TestClass]
public class UserAccountServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Fixture _fixture = new();


    public UserAccountServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
    }

    [TestMethod]
    [DataRow("1000", "1", "100", "10")] // Deposit
    [DataRow("1000.00", "1", "100", "-10")] // Withdraw
    public async Task UpdateAccount_ReturnsAccount(string maxDepositLimitString, string minAccountBalanceString,
        string accountBalanceString, string amountString)
    {
        var maxDepositLimit = decimal.Parse(maxDepositLimitString);
        var minAccountBalance = decimal.Parse(minAccountBalanceString);
        var accountBalance = decimal.Parse(accountBalanceString);
        var amount = decimal.Parse(amountString);

        var options = Options.Create(new AccountOptions
        {
            MaxDepositLimit = maxDepositLimit,
            MinAccountBalance = minAccountBalance,
            StartingAccountBalance = 1
        });
        var sut = new UserAccountService(_userRepositoryMock.Object, _accountRepositoryMock.Object, options);

        var returnedAccount = _fixture.Build<Account>()
            .With(x => x.Balance, accountBalance)
            .Create();

        var expectedUpdatedAccount = returnedAccount with { Balance = returnedAccount.Balance + amount };
        _accountRepositoryMock.Setup(x =>
                x.Update(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns(expectedUpdatedAccount);

        _accountRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAccount);

        var result = await sut.UpdateAccount(returnedAccount.UserId, returnedAccount.AccountId, amount);

        result.Should().BeEquivalentTo(expectedUpdatedAccount);
        _accountRepositoryMock.Verify(x => x.Update(expectedUpdatedAccount.AccountId, expectedUpdatedAccount.UserId,
            expectedUpdatedAccount.Balance - amount, expectedUpdatedAccount.Balance), Times.Once);
    }

    [TestMethod]
    [DataRow("1", "1", "100", "10")] // Deposit
    // Note withdraw has no absolute limit (percentage based), so not testing against that, though should add unit test to get to handle that with comment.
    public async Task UpdateAccount_MaxDepositLimitExceeded_ThrowsAccountException(string maxDepositLimitString,
        string minAccountBalanceString, string accountBalanceString, string amountString)
    {
        var maxDepositLimit = decimal.Parse(maxDepositLimitString);
        var minAccountBalance = decimal.Parse(minAccountBalanceString);
        var accountBalance = decimal.Parse(accountBalanceString);
        var amount = decimal.Parse(amountString);

        var options = Options.Create(new AccountOptions
        {
            MaxDepositLimit = maxDepositLimit,
            MinAccountBalance = minAccountBalance,
            StartingAccountBalance = 1
        });
        var sut = new UserAccountService(_userRepositoryMock.Object, _accountRepositoryMock.Object, options);

        var returnedAccount = _fixture.Build<Account>()
            .With(x => x.Balance, accountBalance)
            .Create();

        _accountRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAccount);

        await FluentActions.Awaiting(() => sut.UpdateAccount(returnedAccount.UserId, returnedAccount.AccountId, amount))
            .Should().ThrowAsync<AccountException>()
            .WithMessage($"You may not deposit more than {maxDepositLimit} in a single transaction");
    }
    
    // Sorry, only got this far due to timme constraint, would basically just be copy of the one above with adjustment for limits and message.
    // I would also change the forbidden exception in the user tests so that one checks the status of the message thrown,
    //  can be done in e.g. API test that handles the throw as it's that layer that deals with it.
}
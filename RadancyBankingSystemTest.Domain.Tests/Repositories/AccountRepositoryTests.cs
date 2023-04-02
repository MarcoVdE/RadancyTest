using AutoFixture;
using FluentAssertions;
using RadancyBankingSystemTest.Domain.Models;
using RadancyBankingSystemTest.Domain.Models.Exceptions;
using RadancyBankingSystemTest.Domain.Repositories;

namespace RadancyBankingSystemTest.Domain.Tests.Repositories;

[TestClass]
public class AccountRepositoryTests
{
    private readonly Fixture _fixture;
    private readonly AccountRepository _sut;

    public AccountRepositoryTests()
    {
        _fixture = new Fixture();
        _sut = new AccountRepository();
    }

    [TestMethod]
    public void Create_AddAccount_ReturnsCorrectGuidForStartingAccount()
    {
        // Arrange
        var expectedAccount = _fixture.Build<Account>()
            .With(x => x.Balance, 1000)
            .With(x => x.UserId, Guid.NewGuid())
            .Create();

        // Act
        var result = _sut.Create(expectedAccount.UserId, expectedAccount.Balance);

        // Assert
        var accountExists = _sut.Accounts.TryGetValue(result, out var account);
        Assert.IsTrue(accountExists);
        account.Should().BeEquivalentTo(expectedAccount, options => options.Excluding(x => x.AccountId));
    }

    [TestMethod]
    public async Task Get_AccountById_ReturnsAccount()
    {
        // Arrange
        var expectedAccount = _fixture.Create<Account>();
        // Usually this would just be a verify if called for amount since e.g. DB request.
        _sut.Accounts.TryAdd(expectedAccount.AccountId, expectedAccount);

        // Act
        var result = await _sut.Get(expectedAccount.AccountId);

        // Assert
        result.Should().BeEquivalentTo(expectedAccount);
    }

    [TestMethod]
    public async Task Get_AccountNotFound_ThrowsKeyNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act and Assert
        await FluentActions.Awaiting(() => _sut.Get(accountId))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Account not found {accountId}");
    }

    [TestMethod]
    public void Update_UpdateAccountBalance_Success()
    {
        // Arrange
        var expectedAccount = _fixture.Create<Account>();
        var valueBefore = expectedAccount.Balance;
        _sut.Accounts.TryAdd(expectedAccount.AccountId, expectedAccount with { }); // with {} is to shallow clone
        expectedAccount.Balance -= 100;

        // Act
        var result = _sut.Update(expectedAccount.AccountId, expectedAccount.UserId, valueBefore,
            expectedAccount.Balance);

        // Assert
        result.Should().BeEquivalentTo(expectedAccount);
    }

    [TestMethod]
    public async Task Update_AccountNotFound_ThrowsKeyNotFound()
    {
        // Arrange
        var expectedAccount = _fixture.Create<Account>();

        // Act and Assert
        await FluentActions.Awaiting(() =>
            {
                _ = _sut.Update(expectedAccount.AccountId, expectedAccount.UserId, 0,
                    expectedAccount.Balance);
                return null;
            })
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Account not found {expectedAccount.AccountId}");
    }

    [TestMethod]
    public async Task Update_AccountDoesNotBelongToUser_ThrowsKeyNotFound()
    {
        // Arrange
        var expectedAccount = _fixture.Create<Account>();
        _sut.Accounts.TryAdd(expectedAccount.AccountId, expectedAccount with { }); // with {} is to shallow clone
        expectedAccount.UserId = Guid.NewGuid();

        // Act and Assert
        await FluentActions.Awaiting(() =>
            {
                _ = _sut.Update(expectedAccount.AccountId, expectedAccount.UserId, 0,
                    expectedAccount.Balance);
                return null;
            })
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Account not found {expectedAccount.AccountId}");
    }

    [TestMethod]
    public async Task Update_StartingBalanceDifferent_ThrowsInvalidAccountStateException()
    {
        // Arrange
        var expectedAccount = _fixture.Create<Account>();
        _sut.Accounts.TryAdd(expectedAccount.AccountId, expectedAccount with { }); // with {} is to shallow clone

        // Act and Assert
        await FluentActions.Awaiting(() =>
            {
                _ = _sut.Update(expectedAccount.AccountId, expectedAccount.UserId, expectedAccount.Balance + 100,
                    expectedAccount.Balance);
                return null;
            })
            .Should().ThrowAsync<AccountException>();
    }

    [TestMethod]
    public void Delete_RemovesAccount()
    {
        // Arrange
        _sut.Accounts.Should().BeEmpty();
        var accountToDelete = _fixture.Create<Account>();

        _sut.Accounts.TryAdd(accountToDelete.AccountId, accountToDelete);
        _sut.Accounts.Should().HaveCount(1);

        // Act:
        _sut.Delete(accountToDelete.AccountId);

        // Assertions
        _sut.Accounts.Should().BeEmpty();
    }

    [TestMethod]
    public void Delete_NoAccount_ShouldNotThrow()
    {
        // Arrange
        _sut.Accounts.Should().BeEmpty();

        // Act and Assert
        FluentActions.Invoking(() => _sut.Delete(Guid.NewGuid()))
            .Should().NotThrow();
    }
}
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RadancyBankingSystemTest.Domain.Models.Exceptions;
using RadancyBankingSystemTest.Domain.Models.Exceptions.Factories;
using RadancyBankingSystemTest.Domain.Repositories;
using RadancyBankingSystemTest.Domain.Repositories.Context;

namespace RadancyBankingSystemTest.Domain.Tests.Repositories;

using Models;

[TestClass]
public class UserRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly Random _random = new();
    
    // This test method I wrote with DBContext directly, this is more in the lines of an integration test based on:
    //  https://stackoverflow.com/posts/54220067/revisions
    // Personally I prefer to moq completely rather than rely on DBContext
    [TestMethod]
    [DataRow(10)]
    public async Task Get_User_ReturnsUser(int numberOfUsersToGenerate)
    {
        // Arrange
        var userOptions = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "Users")
            .Options;
        
        var users = new List<User>();
        await using var context = new UserContext(userOptions);
        
        while (users.Count <= numberOfUsersToGenerate)
        {
            users.Add(_fixture.Create<User>());
        }
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        
        var sut = new UserRepository(context);
        
        var expected = users[_random.Next(numberOfUsersToGenerate+1)];

        // Act
        var result = await sut.Get(expected.UserId);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task Get_UserDoesNotExist_ReturnsKeyNotFound()
    {
        // Arrange
        var userOptions = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "Users")
            .Options;
        
        await using var context = new UserContext(userOptions);
        
        var sut = new UserRepository(context);

        // Act and Assert
        await FluentActions.Awaiting(() => sut.Get(Guid.NewGuid()))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    // Wrote this one more in-line with what I'd usually do, prefer using Moq of the interface.
    [TestMethod]
    public async Task Create_User_ReturnsGuid()
    {
        // Arrange
        var expectedUser = _fixture.Create<User>();
        
        var userOptions = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "Users")
            .Options;
        
        await using var context = new UserContext(userOptions);

        var sut = new UserRepository(context);

        // Act
        var result = await sut.Create(expectedUser.IdNumber!, expectedUser.FirstName, expectedUser.LastName);
        
        // Assert
        var entry = await context.Users.SingleAsync(x => x.IdNumber == expectedUser.IdNumber);

        entry!.UserId.Should().Be(result);
        entry.Should().BeEquivalentTo(expectedUser, options =>
        
            options.Excluding(user => user.UserId)
        );
    }
    
    [TestMethod]
    public async Task Create_UserAlreadyExists_ThrowUserAlreadyRegisteredException()
    {
        // Arrange
        var expectedUser = _fixture.Create<User>();
        
        var userOptions = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "Users")
            .Options;
        
        await using var context = new UserContext(userOptions);
        context.Users.Add(expectedUser);
        await context.SaveChangesAsync();
        
        var expectedError = UserExceptionFactory.UserAlreadyRegisteredException(expectedUser.IdNumber!); 

        var sut = new UserRepository(context);

        // Act and Assert
        await FluentActions.Awaiting(() =>
                sut.Create(expectedUser.IdNumber!, expectedUser.FirstName, expectedUser.LastName))
            .Should().ThrowAsync<UserException>().WithMessage(expectedError.Message);
    }
}
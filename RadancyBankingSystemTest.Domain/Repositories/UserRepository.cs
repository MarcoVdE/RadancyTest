using Microsoft.EntityFrameworkCore;

namespace RadancyBankingSystemTest.Domain.Repositories;

using Context;
using Interfaces;
using Models;
using Models.Exceptions;
using Models.Exceptions.Factories;

public class UserRepository : IUserRepository
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a user
    /// </summary>
    /// <param name="idNumber"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    /// <exception cref="UserException"></exception>
    public async Task<Guid> Create(string idNumber, string firstName, string lastName)
    {
        // Business rule: A customer may only have one user, defined via idNumber 
        // This would have been better as a sqlite backed DB since relational and correctly set up PK/uniqe index.
        var user = await _context.Users.FirstOrDefaultAsync(x => x.IdNumber == idNumber);
        if (user is not null)
        {
            throw UserExceptionFactory.UserAlreadyRegisteredException(idNumber);
        }

        // Could do a search for the user first, would be better, but Guid has a very low chance of hitting same user,
        //  in DB PK would sort this out, so not handling it here as mini test
        var userId = Guid.NewGuid();
        var result = await _context.AddAsync(new User(userId, firstName, lastName, idNumber));
        await _context.SaveChangesAsync();
        return result.Entity.UserId;
    }

    /// <summary>
    /// Get a user, if not found, it will throw an exception.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<User> Get(Guid guid)
    {
        var user = await _context.FindAsync<User>(guid);
        if (user is null) throw new KeyNotFoundException();
        return user;
    }
}
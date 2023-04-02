namespace RadancyBankingSystemTest.Domain.Interfaces;

using Models;

public interface IUserRepository
{
    /// <inheritdoc cref="Repositories.UserRepository.Create"/>
    Task<Guid> Create(string idNumber, string firstName, string lastName);
    /// <inheritdoc cref="Repositories.UserRepository.Get"/>
    public Task<User> Get(Guid guid);
}
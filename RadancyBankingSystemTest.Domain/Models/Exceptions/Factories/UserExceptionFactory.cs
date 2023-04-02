namespace RadancyBankingSystemTest.Domain.Models.Exceptions.Factories;

using System.Net;

public static class UserExceptionFactory
{
    public static UserException UserAlreadyRegisteredException(string idNumber) =>
        new($"A user with the id number {idNumber} has already been registered");
    
    // Since not used, would usually remove, better to remove unused code than keep on off-chance,
    //  get from git history if needed, just showing for example in AccountRepository:55
    public static UserException ForbiddenException(Guid userId) => 
        new($"The user (UserId: {userId}) is forbidden from accessing this resource", HttpStatusCode.Forbidden);
}
namespace RadancyBankingSystemTest.Domain.Models;

public record User(Guid UserId, string FirstName, string LastName, string? IdNumber)
{
    public Guid UserId { get; set; } = UserId;
    public string FirstName { get; set; } = FirstName;
    public string LastName { get; set; } = LastName;

    /// <summary>
    /// This is the user's id number from e.g. drivers/passport, this is to prevent duplicate accounts for the same user.
    /// This should only be used during creation of the account and first time verification.
    /// Usually wouldn't really add this here, wouldn't want to populate it unless needed,
    ///     e.g. lazy load get extended user including id number.
    /// </summary>
    public string? IdNumber { get; set; } = IdNumber;
}
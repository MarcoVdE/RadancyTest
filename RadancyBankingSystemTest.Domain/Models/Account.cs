namespace RadancyBankingSystemTest.Domain.Models;

public record Account(Guid UserId, Guid AccountId, decimal Balance)
{
    public Guid UserId { get; set; } = UserId;
    public Guid AccountId { get; set; } = AccountId;
    public decimal Balance { get; set; } = Balance;
}
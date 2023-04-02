using System.Diagnostics.CodeAnalysis;

namespace RadancyBankingSystemTest.Domain.Configuration;

[ExcludeFromCodeCoverage]
public record AccountOptions
{
    public decimal MinAccountBalance { get; set; }
    public decimal MaxDepositLimit { get; set; }
    public decimal StartingAccountBalance { get; set; }
}
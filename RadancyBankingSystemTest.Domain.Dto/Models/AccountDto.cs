using System.Text.Json.Serialization;

namespace RadancyBankingSystemTest.Domain.Dto.Models;

[Serializable]
public record AccountDto
{
    [JsonConstructor]
    public AccountDto(Guid accountId, Guid userId, double balance)
    {
        AccountId = accountId;
        UserId = userId;
        Balance = balance;
    }
    
    [JsonPropertyName("AccountId")] 
    public Guid AccountId { get; set; }
    
    [JsonPropertyName("UserId")] 
    public Guid UserId  { get; set; }
    
    [JsonPropertyName("Balance")]
    public double Balance { get; set; }
}
using System.Text.Json.Serialization;

namespace RadancyBankingSystemTest.Domain.Dto.Models;

[Serializable]
public record UserDto
{
    [JsonConstructor]
    public UserDto(Guid userId, string firstName, string lastName, string? idNumber)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        IdNumber = idNumber;
    }

    [JsonPropertyName("UserId")] 
    public Guid UserId { get; set; }
    [JsonPropertyName("FirstName")]
    public string FirstName { get; set; }
    [JsonPropertyName("LastName")] 
    public string LastName { get; set; }
    [JsonPropertyName("IdNumber")] 
    public string? IdNumber { get; set; }
}
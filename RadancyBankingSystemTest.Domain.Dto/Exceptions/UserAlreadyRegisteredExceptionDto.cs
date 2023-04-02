using System.Text.Json.Serialization;

namespace RadancyBankingSystemTest.Domain.Dto.Exceptions;

[Serializable]
public class UserExceptionDto : Exception
{
    [JsonPropertyName("StatusCode")] 
    public int StatusCode { get; init; }
    public UserExceptionDto(string? message) : base(message)
    {
    }

    public static UserExceptionDto UserAlreadyRegisteredExceptionDto (string idNumber) =>
        new UserExceptionDto($"A user with the id number {idNumber} has already been registered");
}
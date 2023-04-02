using System.Text.Json.Serialization;

namespace RadancyBankingSystemTest.Domain.Dto.Exceptions;

[Serializable]
public class AccountExceptionDto : Exception
{
    // Just commenting this once here, the idea behind Dto version of exception is in case we have internal objects
    //  that have private user info in them, so that we don't accidentally log private info
    // Could also make a common exception dto to make mapping easier
    [JsonPropertyName("StatusCode")] 
    public int StatusCode { get; init; }

    public AccountExceptionDto(string? message) : base(message)
    {
    }
}
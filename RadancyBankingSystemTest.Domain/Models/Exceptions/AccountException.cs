using System.Net;

namespace RadancyBankingSystemTest.Domain.Models.Exceptions;

public class AccountException : Exception
{
    public HttpStatusCode StatusCode { get; }
    
    internal AccountException(string? message) : base (message)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
    
    public AccountException(string? message, HttpStatusCode statusCode) : base (message)
    {
        StatusCode = statusCode;
    }
}
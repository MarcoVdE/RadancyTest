using System.Net;

namespace RadancyBankingSystemTest.Domain.Models.Exceptions;

public class UserException : Exception
{
    public HttpStatusCode StatusCode { get; init; }
    
    internal UserException(string? message) : base(message)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
    
    internal UserException(string? message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
using System.Net;

namespace BuildingBlocks.Exceptions;

public class HttpException : Exception
{
    public HttpException(HttpStatusCode statusCode, string message)
        : base(string.IsNullOrEmpty(message) ? statusCode.ToString() : message)
    {
        
    }
    
    public HttpException(HttpStatusCode statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        HttpStatusCode = statusCode;
    }

    public HttpStatusCode HttpStatusCode { get; }
}
using System.Net;

namespace BuildingBlocks.Exceptions;

public class BadRequestException : HttpException
{
    public BadRequestException(string message) : base(HttpStatusCode.NotFound, message)
    {
    }

    public BadRequestException(string message, Exception exception) : base(HttpStatusCode.NotFound, message, exception)
    {
    }
}
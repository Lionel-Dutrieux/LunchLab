using System.Net;

namespace PayloadClient.Exceptions;

public class PayloadException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public PayloadException(string message) : base(message)
    {
    }

    public PayloadException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public PayloadException(string message, HttpStatusCode statusCode) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class PayloadDeserializationException : PayloadException
{
    public string? JsonContent { get; }
    public Type? TargetType { get; }

    public PayloadDeserializationException(string message, string? jsonContent, Type? targetType, Exception innerException) 
        : base(message, innerException)
    {
        JsonContent = jsonContent;
        TargetType = targetType;
    }
}

public class PayloadNotFoundException : PayloadException
{
    public PayloadNotFoundException(string message) : base(message, HttpStatusCode.NotFound)
    {
    }
}
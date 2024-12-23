using Microsoft.Extensions.Logging;
using PayloadClient.Exceptions;

public static class LoggerExtensions
{
    public static void LogDeserializationError(
        this ILogger logger,
        PayloadDeserializationException ex)
    {
        logger.LogError(
            ex,
            "Deserialization failed for type {Type}. Content: {Content}",
            ex.TargetType?.Name,
            ex.JsonContent?.Length > 1000 ? ex.JsonContent[..1000] + "..." : ex.JsonContent);
    }
} 
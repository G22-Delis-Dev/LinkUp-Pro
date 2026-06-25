namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public class InvalidImageException : Exception
{
    public InvalidImageException(string message) : base(message)
    {
    }

    public InvalidImageException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

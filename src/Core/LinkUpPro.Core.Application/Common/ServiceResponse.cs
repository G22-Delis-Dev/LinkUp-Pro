namespace LinkUpPro.Application.Common;

// Respuesta estándar de servicios. Encapsula éxito/error con datos tipados
public class ServiceResponse<T>
{
    public bool HasError { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }

    public static ServiceResponse<T> Success(T data) => new()
    {
        HasError = false,
        Data = data
    };

    public static ServiceResponse<T> Failure(string error) => new()
    {
        HasError = true,
        Error = error
    };
}

namespace LinkUpPro.Application.Common;

// Resultado simple para operaciones sin dato de retorno
public class BaseResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();

    public static BaseResult Ok() => new() { Success = true };

    public static BaseResult Fail(string error) => new()
    {
        Success = false,
        Errors = new List<string> { error }
    };

    public static BaseResult Fail(List<string> errors) => new()
    {
        Success = false,
        Errors = errors
    };
}

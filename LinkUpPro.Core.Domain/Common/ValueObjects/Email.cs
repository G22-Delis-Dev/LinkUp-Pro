namespace LinkUpPro.Domain.Common.ValueObjects;

public record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
            throw new ArgumentException("Email inválido.");
        return new Email(value);
    }
}
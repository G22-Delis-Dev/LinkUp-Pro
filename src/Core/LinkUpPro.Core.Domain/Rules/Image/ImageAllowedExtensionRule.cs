namespace LinkUpPro.Domain.Rules.Image;
public class ImageAllowedExtensionRule(string extension) : Common.IBusinessRule
{
    private readonly string[] _allowed = { ".jpg", ".jpeg", ".png", ".webp" };
    public string Message => "El formato de la imagen no está permitido.";
    public bool IsBroken() => !_allowed.Contains(extension.ToLowerInvariant());
}
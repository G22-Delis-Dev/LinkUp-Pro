namespace LinkUpPro.Domain.Rules.Image;

public class ImageMaxSizeRule(long fileSize, long maxBytes = 5242880) : Common.IBusinessRule // 5MB default
{
    public string Message => "La imagen excede el tamaño máximo permitido.";
    public bool IsBroken() => fileSize > maxBytes;
}
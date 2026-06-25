namespace LinkUpPro.Domain.Rules.Image;
public class ImageMustBeValidContentRule(bool isValid) : Common.IBusinessRule
{
    public string Message => "El contenido del archivo no es una imagen válida.";
    public bool IsBroken() => !isValid;
}
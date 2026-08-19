using System.Net;
using System.Text.RegularExpressions;

namespace LinkUpPro.Infrastructure.Shared.Helpers;

// Helper de utilidades para cadenas de texto.
public static class StringHelper
{
    // Trunca un texto a la longitud indicada y agrega "..."
    public static string Truncate(string? text, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text.Length <= maxLength
            ? text
            : string.Concat(text.AsSpan(0, maxLength).TrimEnd(), "...");
    }

    // Capitaliza la primera letra de un texto.
    public static string Capitalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return char.ToUpper(text[0]) + text[1..].ToLower();
    }

   
    // "Todo texto ingresado por el usuario se procesa como texto plano codificado." 
    public static string SanitizeHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return WebUtility.HtmlEncode(input);
    }

    // Genera un nombre de visualización a partir de nombre y apellido.
    public static string GetDisplayName(string? firstName, string? lastName)
    {
        var parts = new[] { firstName?.Trim(), lastName?.Trim() }
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return string.Join(" ", parts);
    }

    // Genera iniciales del usuario para avatares por defecto.
    public static string GetInitials(string? firstName, string? lastName)
    {
        var first = !string.IsNullOrWhiteSpace(firstName) ? firstName[0].ToString().ToUpper() : "";
        var last = !string.IsNullOrWhiteSpace(lastName) ? lastName[0].ToString().ToUpper() : "";
        return $"{first}{last}";
    }

    // Genera un slug URL-friendly a partir de un texto.
    public static string ToSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var slug = text.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[áàäâ]", "a");
        slug = Regex.Replace(slug, @"[éèëê]", "e");
        slug = Regex.Replace(slug, @"[íìïî]", "i");
        slug = Regex.Replace(slug, @"[óòöô]", "o");
        slug = Regex.Replace(slug, @"[úùüû]", "u");
        slug = Regex.Replace(slug, @"[ñ]", "n");
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}

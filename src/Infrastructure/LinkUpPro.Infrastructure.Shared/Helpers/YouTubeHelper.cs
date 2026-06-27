using System.Text.RegularExpressions;

namespace LinkUpPro.Infrastructure.Shared.Helpers;

// Helper para manejo de URLs de YouTube.
// Extrae IDs de video y genera URLs de embed.
public static class YouTubeHelper
{
    // Patrones de URLs de YouTube soportados:
    // - https://www.youtube.com/watch?v=VIDEO_ID
    // - https://youtu.be/VIDEO_ID
    // - https://www.youtube.com/embed/VIDEO_ID
    // - https://www.youtube.com/v/VIDEO_ID
    // - https://youtube.com/shorts/VIDEO_ID
    private static readonly Regex YoutubeRegex = new(
        @"(?:https?://)?(?:www\.)?(?:youtube\.com/(?:watch\?v=|embed/|v/|shorts/)|youtu\.be/)([a-zA-Z0-9_-]{11})",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    // Extrae el ID de un video de YouTube desde cualquier formato de URL soportado.
    // <returns>ID del video (11 caracteres) o null si la URL no es válida.</returns>
    public static string? ExtractVideoId(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var match = YoutubeRegex.Match(url.Trim());
        return match.Success ? match.Groups[1].Value : null;
    }

    // Genera la URL de embed para incrustar el reproductor de YouTube.
    public static string? GetEmbedUrl(string url)
    {
        var videoId = ExtractVideoId(url);
        return videoId != null ? $"https://www.youtube.com/embed/{videoId}" : null;
    }
    public static string? GetThumbnailUrl(string url)
    {
        var videoId = ExtractVideoId(url);
        return videoId != null ? $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg" : null;
    }

    // Verifica si una URL es un enlace válido de YouTube.
    public static bool IsYouTubeUrl(string url)
    {
        return ExtractVideoId(url) != null;
    }
}

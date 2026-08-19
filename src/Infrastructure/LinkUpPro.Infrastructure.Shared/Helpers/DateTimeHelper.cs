namespace LinkUpPro.Infrastructure.Shared.Helpers;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo DefaultTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Santo_Domingo");

    public static DateTime UtcNow() => DateTime.UtcNow;
    public static DateTime ToLocal(DateTime utcDateTime, string? timeZoneId = null)
    {
        var tz = timeZoneId != null
            ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
            : DefaultTimeZone;

        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), tz);
    }

    public static DateTime ToUtc(DateTime localDateTime, string? timeZoneId = null)
    {
        var tz = timeZoneId != null
            ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
            : DefaultTimeZone;

        return TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified), tz);
    }

    public static string GetTimeAgo(DateTime utcDateTime)
    {
        var diff = DateTime.UtcNow - utcDateTime;

        if (diff.TotalSeconds < 60)
            return "Hace un momento";
        if (diff.TotalMinutes < 60)
            return $"Hace {(int)diff.TotalMinutes} {Pluralize((int)diff.TotalMinutes, "minuto", "minutos")}";
        if (diff.TotalHours < 24)
            return $"Hace {(int)diff.TotalHours} {Pluralize((int)diff.TotalHours, "hora", "horas")}";
        if (diff.TotalDays < 7)
            return $"Hace {(int)diff.TotalDays} {Pluralize((int)diff.TotalDays, "día", "días")}";
        if (diff.TotalDays < 30)
        {
            var weeks = (int)(diff.TotalDays / 7);
            return $"Hace {weeks} {Pluralize(weeks, "semana", "semanas")}";
        }
        if (diff.TotalDays < 365)
        {
            var months = (int)(diff.TotalDays / 30);
            return $"Hace {months} {Pluralize(months, "mes", "meses")}";
        }

        var years = (int)(diff.TotalDays / 365);
        return $"Hace {years} {Pluralize(years, "año", "años")}";
    }

    public static string GetElapsedHours(DateTime utcStartDate)
    {
        var diff = DateTime.UtcNow - utcStartDate;

        if (diff.TotalHours < 1)
            return $"{(int)diff.TotalMinutes} min";

        return $"{(int)diff.TotalHours}h {diff.Minutes}m";
    }

    private static string Pluralize(int count, string singular, string plural)
    {
        return count == 1 ? singular : plural;
    }
}

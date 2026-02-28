using System.Globalization;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for DateTime manipulation.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the start of the day (00:00:00).
    /// </summary>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Returns the end of the day (23:59:59.9999999).
    /// </summary>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Returns the start of the week (Monday by default).
    /// </summary>
    public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// Returns the end of the week.
    /// </summary>
    public static DateTime EndOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        return date.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
    }

    /// <summary>
    /// Returns the start of the month.
    /// </summary>
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind);
    }

    /// <summary>
    /// Returns the end of the month.
    /// </summary>
    public static DateTime EndOfMonth(this DateTime date)
    {
        return date.StartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Returns the start of the quarter.
    /// </summary>
    public static DateTime StartOfQuarter(this DateTime date)
    {
        var quarterMonth = ((date.Month - 1) / 3) * 3 + 1;
        return new DateTime(date.Year, quarterMonth, 1, 0, 0, 0, date.Kind);
    }

    /// <summary>
    /// Returns the end of the quarter.
    /// </summary>
    public static DateTime EndOfQuarter(this DateTime date)
    {
        return date.StartOfQuarter().AddMonths(3).AddTicks(-1);
    }

    /// <summary>
    /// Returns the start of the year.
    /// </summary>
    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1, 0, 0, 0, date.Kind);
    }

    /// <summary>
    /// Returns the end of the year.
    /// </summary>
    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31, 23, 59, 59, 999, date.Kind).AddTicks(9999);
    }

    /// <summary>
    /// Calculates the age in years from the birth date.
    /// </summary>
    public static int Age(this DateTime birthDate)
    {
        return birthDate.Age(DateTime.Today);
    }

    /// <summary>
    /// Calculates the age in years from the birth date as of a specific date.
    /// </summary>
    public static int Age(this DateTime birthDate, DateTime asOf)
    {
        var age = asOf.Year - birthDate.Year;
        if (birthDate.Date > asOf.AddYears(-age))
            age--;
        return age;
    }

    /// <summary>
    /// Determines whether the date is today.
    /// </summary>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    /// <summary>
    /// Determines whether the date is yesterday.
    /// </summary>
    public static bool IsYesterday(this DateTime date)
    {
        return date.Date == DateTime.Today.AddDays(-1);
    }

    /// <summary>
    /// Determines whether the date is tomorrow.
    /// </summary>
    public static bool IsTomorrow(this DateTime date)
    {
        return date.Date == DateTime.Today.AddDays(1);
    }

    /// <summary>
    /// Determines whether the date is in the past.
    /// </summary>
    public static bool IsPast(this DateTime date)
    {
        return date < DateTime.Now;
    }

    /// <summary>
    /// Determines whether the date is in the future.
    /// </summary>
    public static bool IsFuture(this DateTime date)
    {
        return date > DateTime.Now;
    }

    /// <summary>
    /// Determines whether the date is between two dates.
    /// </summary>
    public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
    {
        return date >= start && date <= end;
    }

    /// <summary>
    /// Converts the DateTime to a Unix timestamp.
    /// </summary>
    public static long ToUnixTimestamp(this DateTime date)
    {
        return new DateTimeOffset(date.ToUniversalTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converts the DateTime to a Unix timestamp in milliseconds.
    /// </summary>
    public static long ToUnixTimestampMillis(this DateTime date)
    {
        return new DateTimeOffset(date.ToUniversalTime()).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Creates a DateTime from a Unix timestamp.
    /// </summary>
    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }

    /// <summary>
    /// Creates a DateTime from a Unix timestamp in milliseconds.
    /// </summary>
    public static DateTime FromUnixTimestampMillis(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
    }

    /// <summary>
    /// Returns a human-readable relative time string.
    /// </summary>
    public static string ToRelativeTime(this DateTime date, CultureInfo? culture = null)
    {
        var now = DateTime.Now;
        var diff = now - date;
        var isFuture = diff.TotalMilliseconds < 0;
        var absDiff = isFuture ? -diff : diff;

        culture ??= CultureInfo.CurrentCulture;
        var isTurkish = culture.TwoLetterISOLanguageName == "tr";

        string result;

        if (absDiff.TotalSeconds < 60)
        {
            result = isTurkish ? "az önce" : "just now";
        }
        else if (absDiff.TotalMinutes < 60)
        {
            var minutes = (int)absDiff.TotalMinutes;
            result = isTurkish ? $"{minutes} dakika" : $"{minutes} minute{(minutes != 1 ? "s" : "")}";
        }
        else if (absDiff.TotalHours < 24)
        {
            var hours = (int)absDiff.TotalHours;
            result = isTurkish ? $"{hours} saat" : $"{hours} hour{(hours != 1 ? "s" : "")}";
        }
        else if (absDiff.TotalDays < 7)
        {
            var days = (int)absDiff.TotalDays;
            result = isTurkish ? $"{days} gün" : $"{days} day{(days != 1 ? "s" : "")}";
        }
        else if (absDiff.TotalDays < 30)
        {
            var weeks = (int)(absDiff.TotalDays / 7);
            result = isTurkish ? $"{weeks} hafta" : $"{weeks} week{(weeks != 1 ? "s" : "")}";
        }
        else if (absDiff.TotalDays < 365)
        {
            var months = (int)(absDiff.TotalDays / 30);
            result = isTurkish ? $"{months} ay" : $"{months} month{(months != 1 ? "s" : "")}";
        }
        else
        {
            var years = (int)(absDiff.TotalDays / 365);
            result = isTurkish ? $"{years} yıl" : $"{years} year{(years != 1 ? "s" : "")}";
        }

        if (result == (isTurkish ? "az önce" : "just now"))
            return result;

        if (isFuture)
        {
            return isTurkish ? $"{result} sonra" : $"in {result}";
        }

        return isTurkish ? $"{result} önce" : $"{result} ago";
    }

    /// <summary>
    /// Returns a friendly date string.
    /// </summary>
    public static string ToFriendlyString(this DateTime date, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return date.ToString("dddd, d MMMM yyyy", culture);
    }

    /// <summary>
    /// Gets the quarter of the year (1-4).
    /// </summary>
    public static int GetQuarter(this DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    /// <summary>
    /// Gets the week number of the year.
    /// </summary>
    public static int GetWeekOfYear(this DateTime date, CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, rule, firstDayOfWeek);
    }
}

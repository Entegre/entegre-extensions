namespace Entegre.Extensions;

/// <summary>
/// Represents a date range.
/// </summary>
public readonly struct DateRange : IEquatable<DateRange>
{
    /// <summary>
    /// Gets the start date of the range.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the end date of the range.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Creates a new date range.
    /// </summary>
    public DateRange(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ArgumentException("End date must be greater than or equal to start date");

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the duration of the date range.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Gets the number of days in the range.
    /// </summary>
    public int Days => (int)(End.Date - Start.Date).TotalDays + 1;

    /// <summary>
    /// Determines whether the range contains the specified date.
    /// </summary>
    public bool Contains(DateTime date)
    {
        return date >= Start && date <= End;
    }

    /// <summary>
    /// Determines whether this range overlaps with another range.
    /// </summary>
    public bool Overlaps(DateRange other)
    {
        return Start <= other.End && End >= other.Start;
    }

    /// <summary>
    /// Gets the intersection of two ranges.
    /// </summary>
    public DateRange? Intersect(DateRange other)
    {
        if (!Overlaps(other))
            return null;

        return new DateRange(
            Start > other.Start ? Start : other.Start,
            End < other.End ? End : other.End);
    }

    /// <summary>
    /// Gets the union of two ranges if they overlap or are adjacent.
    /// </summary>
    public DateRange? Union(DateRange other)
    {
        if (!Overlaps(other) && Start.Date != other.End.Date.AddDays(1) && End.Date.AddDays(1) != other.Start.Date)
            return null;

        return new DateRange(
            Start < other.Start ? Start : other.Start,
            End > other.End ? End : other.End);
    }

    public bool Equals(DateRange other)
    {
        return Start == other.Start && End == other.End;
    }

    public override bool Equals(object? obj)
    {
        return obj is DateRange other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public static bool operator ==(DateRange left, DateRange right) => left.Equals(right);
    public static bool operator !=(DateRange left, DateRange right) => !left.Equals(right);

    public override string ToString() => $"{Start:yyyy-MM-dd} - {End:yyyy-MM-dd}";
}

/// <summary>
/// Provides extension methods for DateRange operations.
/// </summary>
public static class DateRangeExtensions
{
    /// <summary>
    /// Creates a DateRange from a start date to an end date.
    /// </summary>
    public static DateRange To(this DateTime start, DateTime end)
    {
        return new DateRange(start, end);
    }

    /// <summary>
    /// Gets all dates between the start and end dates.
    /// </summary>
    public static IEnumerable<DateTime> GetDatesBetween(this DateRange range)
    {
        for (var date = range.Start.Date; date <= range.End.Date; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    /// <summary>
    /// Gets all dates between two dates.
    /// </summary>
    public static IEnumerable<DateTime> GetDatesBetween(this DateTime start, DateTime end)
    {
        return new DateRange(start, end).GetDatesBetween();
    }

    /// <summary>
    /// Gets business days in the range (excluding weekends).
    /// </summary>
    public static IEnumerable<DateTime> GetBusinessDays(this DateRange range)
    {
        return range.GetDatesBetween().Where(d => !d.IsWeekend());
    }

    /// <summary>
    /// Gets business days between two dates.
    /// </summary>
    public static IEnumerable<DateTime> GetBusinessDaysBetween(this DateTime start, DateTime end)
    {
        return new DateRange(start, end).GetBusinessDays();
    }

    /// <summary>
    /// Counts business days in the range.
    /// </summary>
    public static int CountBusinessDays(this DateRange range)
    {
        return range.GetBusinessDays().Count();
    }

    /// <summary>
    /// Splits the range into intervals of the specified duration.
    /// </summary>
    public static IEnumerable<DateRange> Split(this DateRange range, TimeSpan interval)
    {
        var current = range.Start;
        while (current < range.End)
        {
            var next = current.Add(interval);
            if (next > range.End)
                next = range.End;

            yield return new DateRange(current, next);
            current = next;
        }
    }

    /// <summary>
    /// Splits the range into daily intervals.
    /// </summary>
    public static IEnumerable<DateRange> SplitByDays(this DateRange range, int days = 1)
    {
        return range.Split(TimeSpan.FromDays(days));
    }

    /// <summary>
    /// Splits the range into weekly intervals.
    /// </summary>
    public static IEnumerable<DateRange> SplitByWeeks(this DateRange range)
    {
        return range.Split(TimeSpan.FromDays(7));
    }

    /// <summary>
    /// Splits the range into monthly intervals.
    /// </summary>
    public static IEnumerable<DateRange> SplitByMonths(this DateRange range)
    {
        var current = range.Start;
        while (current < range.End)
        {
            var next = current.AddMonths(1);
            if (next > range.End)
                next = range.End;

            yield return new DateRange(current, next);
            current = next;
        }
    }
}

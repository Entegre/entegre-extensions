namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for workday calculations.
/// </summary>
public static class WorkdayExtensions
{
    /// <summary>
    /// Determines whether the date is a weekend (Saturday or Sunday).
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    /// <summary>
    /// Determines whether the date is a weekday (Monday through Friday).
    /// </summary>
    public static bool IsWeekday(this DateTime date)
    {
        return !date.IsWeekend();
    }

    /// <summary>
    /// Determines whether the date is a workday (weekday and not a holiday).
    /// </summary>
    public static bool IsWorkday(this DateTime date, IHolidayCalendar? calendar = null)
    {
        if (date.IsWeekend())
            return false;

        if (calendar is not null && calendar.IsHoliday(date))
            return false;

        return true;
    }

    /// <summary>
    /// Determines whether the date is a holiday.
    /// </summary>
    public static bool IsHoliday(this DateTime date, IHolidayCalendar calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        return calendar.IsHoliday(date);
    }

    /// <summary>
    /// Gets the next workday.
    /// </summary>
    public static DateTime NextWorkday(this DateTime date, IHolidayCalendar? calendar = null)
    {
        var next = date.AddDays(1);
        while (!next.IsWorkday(calendar))
        {
            next = next.AddDays(1);
        }

        return next;
    }

    /// <summary>
    /// Gets the previous workday.
    /// </summary>
    public static DateTime PreviousWorkday(this DateTime date, IHolidayCalendar? calendar = null)
    {
        var previous = date.AddDays(-1);
        while (!previous.IsWorkday(calendar))
        {
            previous = previous.AddDays(-1);
        }

        return previous;
    }

    /// <summary>
    /// Adds the specified number of workdays to the date.
    /// </summary>
    public static DateTime AddWorkdays(this DateTime date, int days, IHolidayCalendar? calendar = null)
    {
        var result = date;
        var direction = days >= 0 ? 1 : -1;
        var remaining = Math.Abs(days);

        while (remaining > 0)
        {
            result = result.AddDays(direction);
            if (result.IsWorkday(calendar))
            {
                remaining--;
            }
        }

        return result;
    }

    /// <summary>
    /// Counts the workdays between two dates.
    /// </summary>
    public static int GetWorkdaysBetween(this DateTime start, DateTime end, IHolidayCalendar? calendar = null)
    {
        if (end < start)
        {
            (start, end) = (end, start);
        }

        var count = 0;
        var current = start.Date;

        while (current <= end.Date)
        {
            if (current.IsWorkday(calendar))
            {
                count++;
            }

            current = current.AddDays(1);
        }

        return count;
    }

    /// <summary>
    /// Gets all workdays between two dates.
    /// </summary>
    public static IEnumerable<DateTime> GetWorkdays(this DateTime start, DateTime end, IHolidayCalendar? calendar = null)
    {
        if (end < start)
        {
            (start, end) = (end, start);
        }

        var current = start.Date;

        while (current <= end.Date)
        {
            if (current.IsWorkday(calendar))
            {
                yield return current;
            }

            current = current.AddDays(1);
        }
    }

    /// <summary>
    /// Gets the workday on or after the specified date.
    /// </summary>
    public static DateTime GetWorkdayOrNext(this DateTime date, IHolidayCalendar? calendar = null)
    {
        return date.IsWorkday(calendar) ? date : date.NextWorkday(calendar);
    }

    /// <summary>
    /// Gets the workday on or before the specified date.
    /// </summary>
    public static DateTime GetWorkdayOrPrevious(this DateTime date, IHolidayCalendar? calendar = null)
    {
        return date.IsWorkday(calendar) ? date : date.PreviousWorkday(calendar);
    }
}

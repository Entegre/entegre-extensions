namespace Entegre.Extensions;

/// <summary>
/// Represents a holiday.
/// </summary>
public record Holiday(
    string Name,
    DateTime Date,
    bool IsPublic = true,
    bool IsObserved = true
);

/// <summary>
/// Interface for holiday calendars.
/// </summary>
public interface IHolidayCalendar
{
    /// <summary>
    /// Gets the country code for this calendar.
    /// </summary>
    string Country { get; }

    /// <summary>
    /// Gets all holidays for the specified year.
    /// </summary>
    IEnumerable<Holiday> GetHolidays(int year);

    /// <summary>
    /// Determines whether the specified date is a holiday.
    /// </summary>
    bool IsHoliday(DateTime date);

    /// <summary>
    /// Determines whether the specified date is an observed holiday.
    /// </summary>
    bool IsObservedHoliday(DateTime date);

    /// <summary>
    /// Gets the holiday for the specified date, if any.
    /// </summary>
    Holiday? GetHoliday(DateTime date);
}

/// <summary>
/// Base class for holiday calendars.
/// </summary>
public abstract class HolidayCalendarBase : IHolidayCalendar
{
    private readonly Dictionary<int, List<Holiday>> _cache = new();

    public abstract string Country { get; }

    public abstract IEnumerable<Holiday> GetHolidays(int year);

    public bool IsHoliday(DateTime date)
    {
        return GetHolidaysForYear(date.Year).Any(h => h.Date.Date == date.Date);
    }

    public bool IsObservedHoliday(DateTime date)
    {
        return GetHolidaysForYear(date.Year).Any(h => h.Date.Date == date.Date && h.IsObserved);
    }

    public Holiday? GetHoliday(DateTime date)
    {
        return GetHolidaysForYear(date.Year).FirstOrDefault(h => h.Date.Date == date.Date);
    }

    protected List<Holiday> GetHolidaysForYear(int year)
    {
        if (!_cache.TryGetValue(year, out var holidays))
        {
            holidays = GetHolidays(year).ToList();
            _cache[year] = holidays;
        }

        return holidays;
    }
}

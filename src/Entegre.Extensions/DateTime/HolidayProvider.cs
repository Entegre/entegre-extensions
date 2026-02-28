namespace Entegre.Extensions;

/// <summary>
/// Provides access to holiday calendars.
/// </summary>
public static class HolidayProvider
{
    private static readonly Dictionary<string, IHolidayCalendar> _calendars = new(StringComparer.OrdinalIgnoreCase);

    static HolidayProvider()
    {
        // Register built-in calendars
        Register(new TurkeyHolidayCalendar());
        Register(new USHolidayCalendar());
    }

    /// <summary>
    /// Gets the calendar for Turkey.
    /// </summary>
    public static IHolidayCalendar Turkey => Get("TR");

    /// <summary>
    /// Gets the calendar for the United States.
    /// </summary>
    public static IHolidayCalendar US => Get("US");

    /// <summary>
    /// Gets a holiday calendar by country code.
    /// </summary>
    public static IHolidayCalendar Get(string countryCode)
    {
        if (_calendars.TryGetValue(countryCode, out var calendar))
            return calendar;

        throw new ArgumentException($"No holiday calendar registered for country: {countryCode}");
    }

    /// <summary>
    /// Tries to get a holiday calendar by country code.
    /// </summary>
    public static bool TryGet(string countryCode, out IHolidayCalendar? calendar)
    {
        return _calendars.TryGetValue(countryCode, out calendar);
    }

    /// <summary>
    /// Registers a custom holiday calendar.
    /// </summary>
    public static void Register(IHolidayCalendar calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        _calendars[calendar.Country] = calendar;
    }

    /// <summary>
    /// Gets all registered calendars.
    /// </summary>
    public static IReadOnlyCollection<IHolidayCalendar> GetAll()
    {
        return _calendars.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets all registered country codes.
    /// </summary>
    public static IReadOnlyCollection<string> GetCountryCodes()
    {
        return _calendars.Keys.ToList().AsReadOnly();
    }
}

namespace Entegre.Extensions;

/// <summary>
/// Holiday calendar for Turkey.
/// </summary>
public class TurkeyHolidayCalendar : HolidayCalendarBase
{
    public override string Country => "TR";

    public override IEnumerable<Holiday> GetHolidays(int year)
    {
        // Fixed holidays
        yield return new Holiday("Yılbaşı", new DateTime(year, 1, 1));
        yield return new Holiday("Ulusal Egemenlik ve Çocuk Bayramı", new DateTime(year, 4, 23));
        yield return new Holiday("Emek ve Dayanışma Günü", new DateTime(year, 5, 1));
        yield return new Holiday("Atatürk'ü Anma, Gençlik ve Spor Bayramı", new DateTime(year, 5, 19));
        yield return new Holiday("Zafer Bayramı", new DateTime(year, 8, 30));
        yield return new Holiday("Cumhuriyet Bayramı", new DateTime(year, 10, 29));

        // Islamic holidays (calculated from Hijri calendar)
        foreach (var holiday in GetIslamicHolidays(year))
        {
            yield return holiday;
        }
    }

    private IEnumerable<Holiday> GetIslamicHolidays(int year)
    {
        // Ramazan Bayramı (Eid al-Fitr) - 1 Shawwal
        // Kurban Bayramı (Eid al-Adha) - 10 Dhu al-Hijjah

        // Pre-calculated dates for common years (since Hijri calculation is complex)
        // These are approximate and should be verified for actual use
        var ramazanDates = new Dictionary<int, DateTime>
        {
            { 2024, new DateTime(2024, 4, 10) },
            { 2025, new DateTime(2025, 3, 30) },
            { 2026, new DateTime(2026, 3, 20) },
            { 2027, new DateTime(2027, 3, 9) },
            { 2028, new DateTime(2028, 2, 26) },
            { 2029, new DateTime(2029, 2, 14) },
            { 2030, new DateTime(2030, 2, 4) }
        };

        var kurbanDates = new Dictionary<int, DateTime>
        {
            { 2024, new DateTime(2024, 6, 16) },
            { 2025, new DateTime(2025, 6, 6) },
            { 2026, new DateTime(2026, 5, 27) },
            { 2027, new DateTime(2027, 5, 16) },
            { 2028, new DateTime(2028, 5, 5) },
            { 2029, new DateTime(2029, 4, 24) },
            { 2030, new DateTime(2030, 4, 13) }
        };

        if (ramazanDates.TryGetValue(year, out var ramazanStart))
        {
            // Ramazan Bayramı is 3 days
            yield return new Holiday("Ramazan Bayramı Arifesi", ramazanStart.AddDays(-1));
            yield return new Holiday("Ramazan Bayramı 1. Gün", ramazanStart);
            yield return new Holiday("Ramazan Bayramı 2. Gün", ramazanStart.AddDays(1));
            yield return new Holiday("Ramazan Bayramı 3. Gün", ramazanStart.AddDays(2));
        }

        if (kurbanDates.TryGetValue(year, out var kurbanStart))
        {
            // Kurban Bayramı is 4 days
            yield return new Holiday("Kurban Bayramı Arifesi", kurbanStart.AddDays(-1));
            yield return new Holiday("Kurban Bayramı 1. Gün", kurbanStart);
            yield return new Holiday("Kurban Bayramı 2. Gün", kurbanStart.AddDays(1));
            yield return new Holiday("Kurban Bayramı 3. Gün", kurbanStart.AddDays(2));
            yield return new Holiday("Kurban Bayramı 4. Gün", kurbanStart.AddDays(3));
        }
    }
}

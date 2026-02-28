namespace Entegre.Extensions;

/// <summary>
/// Holiday calendar for the United States.
/// </summary>
public class USHolidayCalendar : HolidayCalendarBase
{
    public override string Country => "US";

    public override IEnumerable<Holiday> GetHolidays(int year)
    {
        // New Year's Day - January 1
        yield return CreateObservedHoliday("New Year's Day", new DateTime(year, 1, 1));

        // Martin Luther King Jr. Day - Third Monday of January
        yield return new Holiday("Martin Luther King Jr. Day", GetNthDayOfWeek(year, 1, DayOfWeek.Monday, 3));

        // Presidents' Day - Third Monday of February
        yield return new Holiday("Presidents' Day", GetNthDayOfWeek(year, 2, DayOfWeek.Monday, 3));

        // Memorial Day - Last Monday of May
        yield return new Holiday("Memorial Day", GetLastDayOfWeek(year, 5, DayOfWeek.Monday));

        // Juneteenth - June 19
        yield return CreateObservedHoliday("Juneteenth National Independence Day", new DateTime(year, 6, 19));

        // Independence Day - July 4
        yield return CreateObservedHoliday("Independence Day", new DateTime(year, 7, 4));

        // Labor Day - First Monday of September
        yield return new Holiday("Labor Day", GetNthDayOfWeek(year, 9, DayOfWeek.Monday, 1));

        // Columbus Day - Second Monday of October
        yield return new Holiday("Columbus Day", GetNthDayOfWeek(year, 10, DayOfWeek.Monday, 2));

        // Veterans Day - November 11
        yield return CreateObservedHoliday("Veterans Day", new DateTime(year, 11, 11));

        // Thanksgiving Day - Fourth Thursday of November
        yield return new Holiday("Thanksgiving Day", GetNthDayOfWeek(year, 11, DayOfWeek.Thursday, 4));

        // Christmas Day - December 25
        yield return CreateObservedHoliday("Christmas Day", new DateTime(year, 12, 25));
    }

    private static Holiday CreateObservedHoliday(string name, DateTime date)
    {
        // If the holiday falls on Saturday, observe on Friday
        // If it falls on Sunday, observe on Monday
        var observedDate = date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(-1),
            DayOfWeek.Sunday => date.AddDays(1),
            _ => date
        };

        return new Holiday(name, observedDate, IsPublic: true, IsObserved: observedDate != date);
    }

    private static DateTime GetNthDayOfWeek(int year, int month, DayOfWeek dayOfWeek, int n)
    {
        var firstDay = new DateTime(year, month, 1);
        var firstDayOfWeek = (int)firstDay.DayOfWeek;
        var targetDayOfWeek = (int)dayOfWeek;

        var daysToAdd = (targetDayOfWeek - firstDayOfWeek + 7) % 7;
        var result = firstDay.AddDays(daysToAdd + (n - 1) * 7);

        return result;
    }

    private static DateTime GetLastDayOfWeek(int year, int month, DayOfWeek dayOfWeek)
    {
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        var lastDayOfWeek = (int)lastDay.DayOfWeek;
        var targetDayOfWeek = (int)dayOfWeek;

        var daysToSubtract = (lastDayOfWeek - targetDayOfWeek + 7) % 7;
        return lastDay.AddDays(-daysToSubtract);
    }
}

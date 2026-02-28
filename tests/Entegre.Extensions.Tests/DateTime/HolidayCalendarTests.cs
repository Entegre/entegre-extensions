using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.DateTime;

public class HolidayCalendarTests
{
    [Fact]
    public void TurkeyHolidayCalendar_ShouldContainFixedHolidays()
    {
        var calendar = HolidayProvider.Turkey;
        var holidays = calendar.GetHolidays(2024).ToList();

        holidays.Should().Contain(h => h.Name == "Yılbaşı" && h.Date == new System.DateTime(2024, 1, 1));
        holidays.Should().Contain(h => h.Name == "Ulusal Egemenlik ve Çocuk Bayramı" && h.Date == new System.DateTime(2024, 4, 23));
        holidays.Should().Contain(h => h.Name == "Emek ve Dayanışma Günü" && h.Date == new System.DateTime(2024, 5, 1));
        holidays.Should().Contain(h => h.Name.Contains("Gençlik") && h.Date == new System.DateTime(2024, 5, 19));
        holidays.Should().Contain(h => h.Name == "Zafer Bayramı" && h.Date == new System.DateTime(2024, 8, 30));
        holidays.Should().Contain(h => h.Name == "Cumhuriyet Bayramı" && h.Date == new System.DateTime(2024, 10, 29));
    }

    [Fact]
    public void TurkeyHolidayCalendar_ShouldContainReligiousHolidays()
    {
        var calendar = HolidayProvider.Turkey;
        var holidays = calendar.GetHolidays(2024).ToList();

        holidays.Should().Contain(h => h.Name.Contains("Ramazan"));
        holidays.Should().Contain(h => h.Name.Contains("Kurban"));
    }

    [Fact]
    public void USHolidayCalendar_ShouldContainFederalHolidays()
    {
        var calendar = HolidayProvider.US;
        var holidays = calendar.GetHolidays(2024).ToList();

        holidays.Should().Contain(h => h.Name == "New Year's Day");
        holidays.Should().Contain(h => h.Name == "Martin Luther King Jr. Day");
        holidays.Should().Contain(h => h.Name == "Presidents' Day");
        holidays.Should().Contain(h => h.Name == "Memorial Day");
        holidays.Should().Contain(h => h.Name == "Independence Day");
        holidays.Should().Contain(h => h.Name == "Labor Day");
        holidays.Should().Contain(h => h.Name == "Thanksgiving Day");
        holidays.Should().Contain(h => h.Name == "Christmas Day");
    }

    [Fact]
    public void USHolidayCalendar_Thanksgiving_ShouldBeFourthThursday()
    {
        var calendar = HolidayProvider.US;
        var holidays = calendar.GetHolidays(2024).ToList();

        var thanksgiving = holidays.First(h => h.Name == "Thanksgiving Day");

        thanksgiving.Date.DayOfWeek.Should().Be(DayOfWeek.Thursday);
        thanksgiving.Date.Month.Should().Be(11);
        thanksgiving.Date.Day.Should().BeInRange(22, 28); // Fourth Thursday range
    }

    [Fact]
    public void HolidayProvider_ShouldReturnCalendarByCountry()
    {
        var turkey = HolidayProvider.Get("TR");
        var us = HolidayProvider.Get("US");

        turkey.Country.Should().Be("TR");
        us.Country.Should().Be("US");
    }

    [Fact]
    public void HolidayProvider_TryGet_ShouldReturnFalseForUnknownCountry()
    {
        var result = HolidayProvider.TryGet("XX", out var calendar);

        result.Should().BeFalse();
        calendar.Should().BeNull();
    }

    [Fact]
    public void HolidayProvider_GetAll_ShouldReturnAllCalendars()
    {
        var calendars = HolidayProvider.GetAll();

        calendars.Should().HaveCountGreaterOrEqualTo(2);
        calendars.Should().Contain(c => c.Country == "TR");
        calendars.Should().Contain(c => c.Country == "US");
    }

    [Fact]
    public void IsHoliday_ShouldReturnTrueForHolidayDates()
    {
        var calendar = HolidayProvider.Turkey;

        calendar.IsHoliday(new System.DateTime(2024, 4, 23)).Should().BeTrue();
        calendar.IsHoliday(new System.DateTime(2024, 4, 22)).Should().BeFalse();
    }

    [Fact]
    public void GetHoliday_ShouldReturnHolidayDetails()
    {
        var calendar = HolidayProvider.Turkey;

        var holiday = calendar.GetHoliday(new System.DateTime(2024, 4, 23));

        holiday.Should().NotBeNull();
        holiday!.Name.Should().Be("Ulusal Egemenlik ve Çocuk Bayramı");
    }

    [Fact]
    public void GetHoliday_ShouldReturnNullForNonHoliday()
    {
        var calendar = HolidayProvider.Turkey;

        var holiday = calendar.GetHoliday(new System.DateTime(2024, 4, 22));

        holiday.Should().BeNull();
    }
}

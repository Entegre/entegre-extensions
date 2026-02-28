using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.DateTime;

public class WorkdayExtensionsTests
{
    [Theory]
    [InlineData(2024, 6, 15, true)]  // Saturday
    [InlineData(2024, 6, 16, true)]  // Sunday
    [InlineData(2024, 6, 17, false)] // Monday
    public void IsWeekend_ShouldReturnExpectedResult(int year, int month, int day, bool expected)
    {
        var date = new System.DateTime(year, month, day);
        date.IsWeekend().Should().Be(expected);
    }

    [Theory]
    [InlineData(2024, 6, 17, true)]  // Monday
    [InlineData(2024, 6, 15, false)] // Saturday
    public void IsWeekday_ShouldReturnExpectedResult(int year, int month, int day, bool expected)
    {
        var date = new System.DateTime(year, month, day);
        date.IsWeekday().Should().Be(expected);
    }

    [Fact]
    public void IsWorkday_WithoutCalendar_ShouldCheckWeekdayOnly()
    {
        var monday = new System.DateTime(2024, 6, 17);
        var saturday = new System.DateTime(2024, 6, 15);

        monday.IsWorkday().Should().BeTrue();
        saturday.IsWorkday().Should().BeFalse();
    }

    [Fact]
    public void IsWorkday_WithCalendar_ShouldExcludeHolidays()
    {
        var calendar = HolidayProvider.Turkey;
        var holiday = new System.DateTime(2024, 4, 23); // National Sovereignty Day
        var regularDay = new System.DateTime(2024, 4, 22);

        holiday.IsWorkday(calendar).Should().BeFalse();
        regularDay.IsWorkday(calendar).Should().BeTrue();
    }

    [Fact]
    public void IsHoliday_ShouldDetectHolidays()
    {
        var calendar = HolidayProvider.Turkey;
        var holiday = new System.DateTime(2024, 4, 23);
        var regularDay = new System.DateTime(2024, 4, 22);

        holiday.IsHoliday(calendar).Should().BeTrue();
        regularDay.IsHoliday(calendar).Should().BeFalse();
    }

    [Fact]
    public void NextWorkday_ShouldSkipWeekends()
    {
        var friday = new System.DateTime(2024, 6, 14);

        var next = friday.NextWorkday();

        next.DayOfWeek.Should().Be(DayOfWeek.Monday);
        next.Should().Be(new System.DateTime(2024, 6, 17));
    }

    [Fact]
    public void PreviousWorkday_ShouldSkipWeekends()
    {
        var monday = new System.DateTime(2024, 6, 17);

        var previous = monday.PreviousWorkday();

        previous.DayOfWeek.Should().Be(DayOfWeek.Friday);
        previous.Should().Be(new System.DateTime(2024, 6, 14));
    }

    [Fact]
    public void AddWorkdays_ShouldSkipWeekends()
    {
        var friday = new System.DateTime(2024, 6, 14);

        var result = friday.AddWorkdays(3);

        result.Should().Be(new System.DateTime(2024, 6, 19)); // Wednesday
    }

    [Fact]
    public void AddWorkdays_Negative_ShouldSubtractWorkdays()
    {
        var wednesday = new System.DateTime(2024, 6, 19);

        var result = wednesday.AddWorkdays(-3);

        result.Should().Be(new System.DateTime(2024, 6, 14)); // Friday
    }

    [Fact]
    public void GetWorkdaysBetween_ShouldCountWorkdaysOnly()
    {
        var monday = new System.DateTime(2024, 6, 17);
        var friday = new System.DateTime(2024, 6, 21);

        var count = monday.GetWorkdaysBetween(friday);

        count.Should().Be(5); // Mon, Tue, Wed, Thu, Fri
    }

    [Fact]
    public void GetWorkdaysBetween_WithWeekend_ShouldExcludeWeekends()
    {
        var friday = new System.DateTime(2024, 6, 14);
        var tuesday = new System.DateTime(2024, 6, 18);

        var count = friday.GetWorkdaysBetween(tuesday);

        count.Should().Be(3); // Fri, Mon, Tue
    }

    [Fact]
    public void GetWorkdays_ShouldReturnWorkdaysEnumerable()
    {
        var monday = new System.DateTime(2024, 6, 17);
        var nextMonday = new System.DateTime(2024, 6, 24);

        var workdays = monday.GetWorkdays(nextMonday).ToList();

        workdays.Should().HaveCount(6); // Mon-Fri + Mon
        workdays.All(d => !d.IsWeekend()).Should().BeTrue();
    }

    [Fact]
    public void GetWorkdayOrNext_OnWeekend_ShouldReturnNextWorkday()
    {
        var saturday = new System.DateTime(2024, 6, 15);
        var monday = new System.DateTime(2024, 6, 17);

        saturday.GetWorkdayOrNext().Should().Be(monday);
        monday.GetWorkdayOrNext().Should().Be(monday);
    }

    [Fact]
    public void GetWorkdayOrPrevious_OnWeekend_ShouldReturnPreviousWorkday()
    {
        var saturday = new System.DateTime(2024, 6, 15);
        var friday = new System.DateTime(2024, 6, 14);

        saturday.GetWorkdayOrPrevious().Should().Be(friday);
        friday.GetWorkdayOrPrevious().Should().Be(friday);
    }
}

using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.DateTime;

public class DateTimeExtensionsTests
{
    [Fact]
    public void StartOfDay_ShouldReturnMidnight()
    {
        var date = new System.DateTime(2024, 6, 15, 14, 30, 45);

        var result = date.StartOfDay();

        result.Should().Be(new System.DateTime(2024, 6, 15, 0, 0, 0));
    }

    [Fact]
    public void EndOfDay_ShouldReturnLastMoment()
    {
        var date = new System.DateTime(2024, 6, 15, 14, 30, 45);

        var result = date.EndOfDay();

        result.Year.Should().Be(2024);
        result.Month.Should().Be(6);
        result.Day.Should().Be(15);
        result.Hour.Should().Be(23);
        result.Minute.Should().Be(59);
        result.Second.Should().Be(59);
    }

    [Fact]
    public void StartOfWeek_Monday_ShouldReturnMonday()
    {
        var wednesday = new System.DateTime(2024, 6, 12); // A Wednesday

        var result = wednesday.StartOfWeek(DayOfWeek.Monday);

        result.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Should().Be(new System.DateTime(2024, 6, 10));
    }

    [Fact]
    public void StartOfMonth_ShouldReturnFirstDay()
    {
        var date = new System.DateTime(2024, 6, 15);

        var result = date.StartOfMonth();

        result.Should().Be(new System.DateTime(2024, 6, 1));
    }

    [Fact]
    public void EndOfMonth_ShouldReturnLastMoment()
    {
        var date = new System.DateTime(2024, 6, 15);

        var result = date.EndOfMonth();

        result.Day.Should().Be(30);
        result.Hour.Should().Be(23);
    }

    [Fact]
    public void StartOfQuarter_ShouldReturnQuarterStart()
    {
        var date = new System.DateTime(2024, 5, 15); // Q2

        var result = date.StartOfQuarter();

        result.Should().Be(new System.DateTime(2024, 4, 1));
    }

    [Fact]
    public void EndOfQuarter_ShouldReturnQuarterEnd()
    {
        var date = new System.DateTime(2024, 5, 15); // Q2

        var result = date.EndOfQuarter();

        result.Month.Should().Be(6);
        result.Day.Should().Be(30);
    }

    [Fact]
    public void StartOfYear_ShouldReturnJanuaryFirst()
    {
        var date = new System.DateTime(2024, 6, 15);

        var result = date.StartOfYear();

        result.Should().Be(new System.DateTime(2024, 1, 1));
    }

    [Fact]
    public void EndOfYear_ShouldReturnDecemberThirtyFirst()
    {
        var date = new System.DateTime(2024, 6, 15);

        var result = date.EndOfYear();

        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
    }

    [Fact]
    public void Age_ShouldCalculateCorrectly()
    {
        var birthDate = new System.DateTime(1990, 6, 15);
        var today = new System.DateTime(2024, 6, 14);

        var age = birthDate.Age(today);

        age.Should().Be(33); // Not yet 34
    }

    [Fact]
    public void Age_OnBirthday_ShouldBeCorrect()
    {
        var birthDate = new System.DateTime(1990, 6, 15);
        var today = new System.DateTime(2024, 6, 15);

        var age = birthDate.Age(today);

        age.Should().Be(34);
    }

    [Fact]
    public void IsToday_ShouldReturnTrueForToday()
    {
        var today = System.DateTime.Today;

        today.IsToday().Should().BeTrue();
        today.AddDays(-1).IsToday().Should().BeFalse();
    }

    [Fact]
    public void IsYesterday_ShouldReturnTrueForYesterday()
    {
        var yesterday = System.DateTime.Today.AddDays(-1);

        yesterday.IsYesterday().Should().BeTrue();
        System.DateTime.Today.IsYesterday().Should().BeFalse();
    }

    [Fact]
    public void IsTomorrow_ShouldReturnTrueForTomorrow()
    {
        var tomorrow = System.DateTime.Today.AddDays(1);

        tomorrow.IsTomorrow().Should().BeTrue();
        System.DateTime.Today.IsTomorrow().Should().BeFalse();
    }

    [Fact]
    public void IsPast_ShouldReturnTrueForPastDates()
    {
        var past = System.DateTime.Now.AddHours(-1);
        var future = System.DateTime.Now.AddHours(1);

        past.IsPast().Should().BeTrue();
        future.IsPast().Should().BeFalse();
    }

    [Fact]
    public void IsBetween_ShouldCheckRange()
    {
        var start = new System.DateTime(2024, 1, 1);
        var end = new System.DateTime(2024, 12, 31);
        var middle = new System.DateTime(2024, 6, 15);
        var outside = new System.DateTime(2025, 1, 1);

        middle.IsBetween(start, end).Should().BeTrue();
        outside.IsBetween(start, end).Should().BeFalse();
    }

    [Fact]
    public void ToUnixTimestamp_ShouldConvertCorrectly()
    {
        var date = new System.DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var timestamp = date.ToUnixTimestamp();

        timestamp.Should().Be(1704067200);
    }

    [Fact]
    public void FromUnixTimestamp_ShouldConvertCorrectly()
    {
        var timestamp = 1704067200L;

        var date = timestamp.FromUnixTimestamp();

        date.Year.Should().Be(2024);
        date.Month.Should().Be(1);
        date.Day.Should().Be(1);
    }

    [Fact]
    public void ToRelativeTime_English_ShouldFormatCorrectly()
    {
        var now = System.DateTime.Now;
        var culture = new CultureInfo("en-US");

        now.AddMinutes(-5).ToRelativeTime(culture).Should().Contain("minute");
        now.AddHours(-2).ToRelativeTime(culture).Should().Contain("hour");
        now.AddDays(-3).ToRelativeTime(culture).Should().Contain("day");
    }

    [Fact]
    public void ToRelativeTime_Turkish_ShouldFormatCorrectly()
    {
        var now = System.DateTime.Now;
        var culture = new CultureInfo("tr-TR");

        now.AddMinutes(-5).ToRelativeTime(culture).Should().Contain("dakika");
        now.AddHours(-2).ToRelativeTime(culture).Should().Contain("saat");
        now.AddDays(-3).ToRelativeTime(culture).Should().Contain("gün");
    }

    [Fact]
    public void GetQuarter_ShouldReturnCorrectQuarter()
    {
        new System.DateTime(2024, 1, 1).GetQuarter().Should().Be(1);
        new System.DateTime(2024, 4, 1).GetQuarter().Should().Be(2);
        new System.DateTime(2024, 7, 1).GetQuarter().Should().Be(3);
        new System.DateTime(2024, 10, 1).GetQuarter().Should().Be(4);
    }
}

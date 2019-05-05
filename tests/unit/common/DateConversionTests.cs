using System;
using FluentAssertions;
using RiverFlow.Common;
using Xunit;

namespace RiverFlow.Tests.Unit
{
    public class DateConversionTests
    {
        [Fact]
        public void ForStorage_ReturnsUTC()
        {
            var dt = DateTime.Parse("2010-06-02T08:45:00.000-05:00");
            var expected = dt.ToUniversalTime();
            var actual = DateConversion.ForStorage(dt);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ForGaugeSite_EST_ConvertedFromUTC()
        {
            var dt = DateTime.Parse("6/2/2010 1:45:00 PM");
            var expected = DateTime.Parse("6/2/2010 09:45:00 AM");
            var actual = DateConversion.ForGaugeSite(dt, "EST");
            actual.Should().NotBeNull();
            actual.date.Should().NotBeNull();
            actual.date.Should().Be(expected);
            actual.error.Should().BeNull();
        }

        [Fact]
        public void ForGaugeSite_NullZone_ReturnsErrorAndNoDate()
        {
            var dt = DateTime.Parse("6/2/2010 1:45:00 PM");
            var expected = DateTime.Parse("6/2/2010 09:45:00 AM");
            var actual = DateConversion.ForGaugeSite(dt, null);
            actual.Should().NotBeNull();
            actual.error.Should().NotBeNullOrEmpty();
            actual.date.Should().BeNull();
        }

        [Fact]
        public void ForGaugeSite_EmptyZone_ReturnsErrorAndNoDate()
        {
            var dt = DateTime.Parse("6/2/2010 1:45:00 PM");
            var expected = DateTime.Parse("6/2/2010 09:45:00 AM");
            var actual = DateConversion.ForGaugeSite(dt, string.Empty);
            actual.Should().NotBeNull();
            actual.error.Should().NotBeNullOrEmpty();
            actual.date.Should().BeNull();
        }

        [Fact]
        public void ForGaugeSite_BadZone_ReturnsErrorAndNoDate()
        {
            var dt = DateTime.Parse("6/2/2010 1:45:00 PM");
            var expected = DateTime.Parse("6/2/2010 09:45:00 AM");
            var actual = DateConversion.ForGaugeSite(dt, Guid.NewGuid().ToString());
            actual.Should().NotBeNull();
            actual.error.Should().NotBeNullOrEmpty();
            actual.date.Should().BeNull();
        }
    }
}
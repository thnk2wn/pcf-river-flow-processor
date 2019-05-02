using System;
using FluentAssertions;
using RiverFlow.Common;
using Xunit;

namespace RiverFlowProcessor.Tests.Unit
{
    public class UsgsTests
    {
        [Fact]
        public void FormatGaugeId_LessThan8Characters_PadLeftZeros()
        {
            Usgs.FormatGaugeId("3539600").Should().Be("03539600");
        }

        [Fact]
        public void FormatGaugeId_8Characters_NoChange()
        {
            Usgs.FormatGaugeId("03539600").Should().Be("03539600");
        }

        [Fact]
        public void FormatGaugeId_MoreThan8Characters_NoChange()
        {
            Usgs.FormatGaugeId("013539600").Should().Be("013539600");
        }

        [Fact]
        public void FormatGaugeId_Null_ThrowsArgumentNullException()
        {
            Action act = () => Usgs.FormatGaugeId(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FormatGaugeId_Empty_ThrowsArgumentException()
        {
            Action act = () => Usgs.FormatGaugeId(string.Empty);
            act.Should().Throw<ArgumentException>();
        }
    }
}

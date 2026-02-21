using FluentAssertions;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_should_return_null_when_value_is_null_or_whitespace(string? input)
        {
            var phone = PhoneNumber.Create(input);

            phone.Should().BeNull();
        }

        [Fact]
        public void Create_should_return_phone_number_when_value_is_valid_e164()
        {
            var phone = PhoneNumber.Create("+46701234567")!.Value;

            phone.Should().NotBeNull();
            phone!.Value.Should().Be("+46701234567");
        }

        [Fact]
        public void Create_should_trim_and_clean_input_before_validation()
        {
            var phone = PhoneNumber.Create(" +46 70-123 45 67 ");

            phone.Should().NotBeNull();
            phone!.Value.Value.Should().Be("+46701234567");
        }

        [Theory]
        [InlineData("0701234567")]      // missing +
        [InlineData("+0701234567")]     // country code cannot start with 0
        [InlineData("+46")]             // too short
        [InlineData("+461234567890123456")] // too long (>15 digits)
        [InlineData("+46abc123456")]    // invalid characters
        public void Create_should_throw_when_format_is_invalid(string input)
        {
            var act = () => PhoneNumber.Create(input);

            act.Should().Throw<InvalidPhoneNumberException>();
        }

        [Fact]
        public void ToString_should_return_value()
        {
            var phone = PhoneNumber.Create("+46701234567")!.Value;

            phone.ToString().Should().Be("+46701234567");
        }
    }
}

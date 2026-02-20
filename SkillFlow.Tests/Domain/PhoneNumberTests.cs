using FluentAssertions;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Domain
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
        public void Create_should_return_phone_number_when_value_is_present()
        {
            var phone = PhoneNumber.Create("0701234567")!.Value;

            phone.Value.Should().Be("0701234567");
        }

        [Fact]
        public void Create_should_throw_when_value_exceeds_max_length()
        {
            var tooLong = new string('1', PhoneNumber.MaxLength + 1);

            var act = () => PhoneNumber.Create(tooLong);

            act.Should().Throw<InvalidPhoneNumberException>();
        }

        [Fact]
        public void ToString_should_return_value()
        {
            var phone = PhoneNumber.Create("0701234567")!.Value;

            phone.ToString().Should().Be("0701234567");
        }
    }
}

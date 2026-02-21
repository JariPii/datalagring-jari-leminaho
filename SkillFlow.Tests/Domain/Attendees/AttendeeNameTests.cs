using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class AttendeeNameTests
    {
        [Fact]
        public void Create_should_return_cleaned_first_and_last_name()
        {
            var first = "  rOsElLa  ";
            var last = "  gRaDy  ";

            var expectedFirst = first.NormalizeName();
            var expectedLast = last.NormalizeName();

            var name = AttendeeName.Create(first, last);

            name.FirstName.Should().Be(expectedFirst);
            name.LastName.Should().Be(expectedLast);
        }

        [Fact]
        public void Create_should_throw_when_first_name_exceeds_max_length_after_normalization()
        {
            var tooLong = new string('a', AttendeeName.MaxLength + 1);

            var act = () => AttendeeName.Create(tooLong, "Doe");

            act.Should().Throw<InvalidNameException>();
        }

        [Fact]
        public void Create_should_throw_when_last_name_exceeds_max_length_after_normalization()
        {
            var tooLong = new string('b', AttendeeName.MaxLength + 1);

            var act = () => AttendeeName.Create("John", tooLong);

            act.Should().Throw<InvalidNameException>();
        }

        [Theory]
        [InlineData("Jari2", "Doe")]
        [InlineData("John", "D0e")]
        [InlineData("John_", "Doe")]
        [InlineData("John", "Doe@")]
        [InlineData("  ", "Doe")]
        [InlineData("John", "   ")]
        [InlineData("-John", "Doe")]
        [InlineData("John", "Doe-")]
        [InlineData("John--Paul", "Doe")]
        public void Create_should_throw_when_name_contains_invalid_characters(string first, string last)
        {
            var act = () => AttendeeName.Create(first, last);

            act.Should().Throw<InvalidNameException>();
        }

        [Fact]
        public void Fullname_should_be_firstname_space_lastname()
        {
            var name = AttendeeName.Create("John", "Doe");

            name.Fullname.Should().Be("John Doe");
        }

        [Fact]
        public void ToString_should_return_fullname()
        {
            var name = AttendeeName.Create("John", "Doe");

            name.ToString().Should().Be("John Doe");
        }

        [Fact]
        public void Record_struct_should_have_value_equality()
        {
            var a = AttendeeName.Create(" John ", " Doe ");
            var b = AttendeeName.Create("John", "Doe");

            a.Should().Be(b);
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Fullname_should_use_normalized_parts()
        {
            var first = "  jOhN  ";
            var last = "  dOe  ";

            var expected = $"{first.NormalizeName()} {last.NormalizeName()}";

            var name = AttendeeName.Create(first, last);

            name.Fullname.Should().Be(expected);
        }
    }
}

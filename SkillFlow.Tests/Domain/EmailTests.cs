using FluentAssertions;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Domain
{
    public class EmailTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_should_throw_when_value_is_null_or_whitespace(string? input)
        {
            var act = () => Email.Create(input!);

            act.Should().Throw<InvalidEmailException>();
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("a@")]
        [InlineData("@b.com")]
        [InlineData("a@@b.com")]
        [InlineData("a b@c.com")]
        public void Create_should_throw_when_email_format_is_invalid(string input)
        {
            var act = () => Email.Create(input);

            act.Should().Throw<InvalidEmailException>();
        }

        [Fact]
        public void Create_should_trim_and_lowercase_value()
        {
            var email = Email.Create("  TeSt@Example.COM  ");

            email.Value.Should().Be("test@example.com");
        }

        [Fact]
        public void UniqueValue_should_remove_plus_tag_for_any_domain()
        {
            var email = Email.Create("user+tag@mail.com");

            email.UniqueValue.Should().Be("user@mail.com");
        }

        [Theory]
        [InlineData("gmail.com")]
        [InlineData("googlemail.com")]
        public void UniqueValue_should_remove_dots_only_for_gmail_domains(string domain)
        {
            var email = Email.Create($"te.st+tag@{domain}");

            email.UniqueValue.Should().Be($"test@{domain}");
        }

        [Fact]
        public void UniqueValue_should_not_remove_dots_for_non_gmail_domains()
        {
            var email = Email.Create("te.st+tag@mail.com");

            email.UniqueValue.Should().Be("te.st@mail.com");
        }

        [Fact]
        public void ToString_should_return_value()
        {
            var email = Email.Create("Test@Example.com");

            email.ToString().Should().Be("test@example.com");
        }

        [Fact]
        public void UniqueValue_should_be_lowercase()
        {
            var email = Email.Create("Te.St+Tag@Gmail.COM");

            email.UniqueValue.Should().Be("test@gmail.com");
        }

        [Fact]
        public void Equals_should_use_unique_value_for_gmail_variants()
        {
            var a = Email.Create("te.st+tag@gmail.com");
            var b = Email.Create("test@gmail.com");

            a.Should().Be(b);
        }

    }
}

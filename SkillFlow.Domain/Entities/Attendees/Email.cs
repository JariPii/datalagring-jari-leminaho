using SkillFlow.Domain.Exceptions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Entities.Attendees
{
    public readonly partial record struct Email
    {
        public string Value { get; }
        public string UniqueValue { get; }

        private Email(string value, string uniqueValue)
        {
            Value = value;
            UniqueValue = uniqueValue;
        }

        [GeneratedRegex(
            @"^(?=.{3,254}$)(?=.{1,64}@)" +
            @"[\p{L}\p{N}]+([._%+\-][\p{L}\p{N}]+)*" +
            @"@" +
            @"(?:[A-Z0-9](?:[A-Z0-9\-]{0,61}[A-Z0-9])?\.)+" +
            @"(?:[A-Z]{2,63}|XN--[A-Z0-9\-]{2,59})$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex EmailRegex();

        public static Email Create(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailException($"Email is required");

            var trimmed = value.Trim();

            string normalized = NormalizeDomainForValidation(trimmed);

            if (!EmailRegex().IsMatch(normalized))
                throw new InvalidEmailException("Invalid email format");

            var unique = CreateUniqueKey(normalized);

            return new Email(trimmed.ToLowerInvariant(), unique);
        }

        private static string NormalizeDomainForValidation(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 0 || at != email.LastIndexOf('@') || at == email.Length - 1)
                return email;

            var local = email[..at];
            var domain = email[(at + 1)..];

            try
            {
                domain = new IdnMapping().GetAscii(domain);
            }
            catch (ArgumentException) { return email; }

            return $"{local}@{domain}";
        }

        private static string CreateUniqueKey(string normalizedEmail)
        {
            var at = normalizedEmail.IndexOf('@');
            if (at <= 0 || at != normalizedEmail.LastIndexOf('@') || at == normalizedEmail.Length - 1)
                return normalizedEmail.ToLowerInvariant();

            var local = normalizedEmail[..at].ToLowerInvariant();
            var domain = normalizedEmail[(at + 1)..].ToLowerInvariant();

            var plus = local.IndexOf('+');
            if (plus >= 0) local = local[..plus];

            if (domain is "gmail.com" or "googlemail.com")
                local = local.Replace(".", "");

            
            return $"{local}@{domain}";
        }

        public bool Equals(Email other) => UniqueValue == other.UniqueValue;
        public override int GetHashCode() => UniqueValue.GetHashCode();

        public override string ToString() => Value;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Primitives
{
    public static class StringExtensions
    {
        public static string NormalizeName(this string value) => value.Trim();
    }
}

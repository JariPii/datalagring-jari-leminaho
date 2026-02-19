using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SkillFlow.Application.Helpers
{
    public static class EnumDisplyName
    {
        public static string GetDisplayName<T>(T value) where T : struct, Enum
        {
            var name = value.ToString();
            var field = typeof(T).GetField(name);
            var display = field?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? name;
        }
    }
}

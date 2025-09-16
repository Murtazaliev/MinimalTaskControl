using System.ComponentModel;
using System.Reflection;

namespace MinimalTaskControl.Core.Extensions;

public static class EnumExtension
{
    public static string ToEnumDescription<T>(this T enumer) where T : Enum
    {
        Type enumerType = enumer.GetType();
        string enumerValue = enumer.ToString();

        if (string.IsNullOrEmpty(enumerValue))
        {
            return string.Empty;
        }

        var field = enumerType.GetField(enumerValue);
        if (field == null)
        {
            return string.Empty;
        }

        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        if (attr == null)
        {
            return string.Empty;
        }

        return string.IsNullOrEmpty(attr.Description) ? string.Empty : attr.Description;
    }
}
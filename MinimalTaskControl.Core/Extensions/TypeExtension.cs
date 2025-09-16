namespace MinimalTaskControl.Core.Extensions;

public static class TypeExtension
{
    public static string GetGenericTypeName(this Type type)
    {
        string typeName = string.Empty;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(", ", type.GetGenericArguments().Select(t => t.GetGenericTypeName()).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }
}

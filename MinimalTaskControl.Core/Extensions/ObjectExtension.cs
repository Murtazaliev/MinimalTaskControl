namespace MinimalTaskControl.Core.Extensions;

public static class ObjectExtension
{
    public static string GetGenericTypeName(this object obj)
    {
        return obj.GetType().GetGenericTypeName();
    }
}

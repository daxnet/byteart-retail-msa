namespace ByteartRetail.Common;

public static class Utils
{
    public static bool IsSimpleType(this Type src)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        return src.IsEnum ||
               src.IsGenericType &&
               src.GetGenericTypeDefinition() == typeof(Nullable<>) &&
               src.GetGenericArguments().First().IsEnum ||
               SimpleTypesInternal.Value.Contains(src);
    }
    
    private static readonly Lazy<Type[]> SimpleTypesInternal = new(() =>
    {
        var types = new[]
        {
            typeof (Enum),
            typeof (string),
            typeof (char),
            typeof (Guid),

            typeof (bool),
            typeof (byte),
            typeof (short),
            typeof (int),
            typeof (long),
            typeof (float),
            typeof (double),
            typeof (decimal),

            typeof (sbyte),
            typeof (ushort),
            typeof (uint),
            typeof (ulong),

            typeof (DateTime),
            typeof (DateTimeOffset),
            typeof (TimeSpan),
        };


        var nullableTypes = from t in types
            where t != typeof(Enum) && t != typeof(string)
            select typeof(Nullable<>).MakeGenericType(t);

        return types.Concat(nullableTypes).ToArray();
    });
}
namespace FlexiForms.Extensions
{
    public static class NullExtensions
    {
        public static IEnumerable<TSource> EnsureNotNull<TSource>(this IEnumerable<TSource>? source)
        {
            if (source != null)
            {
                return source;
            }

            return new List<TSource>();
        }

        public static T EnsureNotNull<T>(this T? source)
            where T : new()
        {
            if (source != null)
            {
                return source;
            }

            return new T();
        }
    }
}

namespace EnglishClass.Common.Extensions
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string? s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNullOrEmpty(this string? s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool EqualsIgnoreCase(this string? s, string? other)
        {
            if (s is null && other is null)
            {
                return true;
            }

            if ((s is null && other is not null) || (s is not null && other is null))
            {
                return false;
            }

            return s!.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
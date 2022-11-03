using System.Linq;
public static class LinqHelper
{
    public static bool ContainsAny(this string haystack, params string[] needles)
    {
        return needles.Any(c => haystack.Contains(c));
    }
}
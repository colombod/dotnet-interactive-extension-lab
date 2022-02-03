using System.Text.RegularExpressions;

namespace TestUtilities;

public static class StringExtensions
{
    public static string FixedGuid(this string source)
    {
        var reg = new Regex(@".*\s+id=""(?<id>\S+)""\s*.*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        var id1 = reg.Match(source).Groups["id"].Value;
        var id = id1;
        return source.Replace(id, "00000000000000000000000000000000");
    }

    public static string FixedCacheBuster(this string source)
    {
        var reg = new Regex(@".*\s+'cacheBuster=(?<cacheBuster>\S+)'\s*.*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        var id1 = reg.Match(source).Groups["cacheBuster"].Value;
        var id = id1;
        return source.Replace(id, "00000000000000000000000000000000");
    }
}
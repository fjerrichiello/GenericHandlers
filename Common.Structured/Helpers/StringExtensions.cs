using System.Text.RegularExpressions;

namespace Common.Structured.Helpers;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input) => Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
}
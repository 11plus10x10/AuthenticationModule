namespace AuthenticationModule.Utils;

internal static class ExtensionMethods
{
    internal static bool IsNullOrContainsSpecialChars(this string str)
    {
        var specialCharacters = new[]
        {
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', ',', '.', '?', '"', ':', '{', '}', '|', '<', '>', '=',
            '+', '/', '\\', '-', '_', '[', ']'
        };

        return string.IsNullOrWhiteSpace(str) || str.Any(c => specialCharacters.Contains(c));
    }

    internal static bool IsNegative(this int number) => number < 0;
}

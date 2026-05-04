using System.Text.RegularExpressions;

namespace TwilioCallCenter.Common;

public static class PhoneNumberValidator
{
    private static readonly Regex E164 = new(@"^\+[1-9]\d{7,14}$", RegexOptions.Compiled);

    public static bool IsValid(string? number) =>
        !string.IsNullOrWhiteSpace(number) && E164.IsMatch(number);
}

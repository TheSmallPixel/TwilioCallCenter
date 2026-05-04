using System.Security.Cryptography;
using System.Text;

namespace TwilioCallCenter.Tests;

internal static class TwilioSignature
{
    public static string Compute(string authToken, string url, IEnumerable<KeyValuePair<string, string>> formParams)
    {
        var sb = new StringBuilder(url);
        foreach (var pair in formParams.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            sb.Append(pair.Key).Append(pair.Value);
        }
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(authToken));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToBase64String(hash);
    }
}

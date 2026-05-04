using Microsoft.AspNetCore.Http;

namespace TwilioCallCenter.Common;

public static class FormCollectionExtensions
{
    public static IDictionary<string, string> ToStringDictionary(this IFormCollection form) =>
        form.Keys.ToDictionary(k => k, k => form[k].ToString());
}

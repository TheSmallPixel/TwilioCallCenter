using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Twilio.Security;
using TwilioCallCenter.Common;
using TwilioCallCenter.Configuration;

namespace TwilioCallCenter.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateTwilioRequestAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var options = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<TwilioOptions>>().Value;

        if (string.IsNullOrEmpty(options.AuthToken))
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        var request = context.HttpContext.Request;
        var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        var signature = request.Headers["X-Twilio-Signature"].ToString();

        IDictionary<string, string> parameters = new Dictionary<string, string>();
        if (request.HasFormContentType)
        {
            var form = await request.ReadFormAsync();
            parameters = form.ToStringDictionary();
        }

        var validator = new RequestValidator(options.AuthToken);
        if (!validator.Validate(url, parameters, signature))
        {
            context.Result = new StatusCodeResult(403);
            return;
        }

        await next();
    }
}

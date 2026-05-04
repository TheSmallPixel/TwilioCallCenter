using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TwilioCallCenter.Common;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Models;

namespace TwilioCallCenter.Controllers;

[ApiController]
[Route("api/sms")]
public class SmsController : ControllerBase
{
    private readonly TwilioOptions _twilio;

    public SmsController(IOptions<TwilioOptions> twilio)
    {
        _twilio = twilio.Value;
    }

    [HttpPost]
    public IActionResult Send([FromBody] SendSmsRequest? request)
    {
        if (request is null) return BadRequest("body required");
        if (string.IsNullOrWhiteSpace(request.Text)) return BadRequest("text required");
        if (!PhoneNumberValidator.IsValid(request.To)) return BadRequest("invalid 'to' number (E.164 expected)");

        TwilioClient.Init(_twilio.AccountSid, _twilio.AuthToken);
        var message = MessageResource.Create(
            body: request.Text,
            from: new PhoneNumber(_twilio.FromNumber),
            to: new PhoneNumber(request.To));

        return Accepted(new { sid = message.Sid });
    }
}

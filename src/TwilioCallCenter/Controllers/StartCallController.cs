using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TwilioCallCenter.Common;
using TwilioCallCenter.Data;
using TwilioCallCenter.Models;
using TwilioCallCenter.Service;

namespace TwilioCallCenter.Controllers;

[ApiController]
[Route("api/calls")]
public class StartCallController : ControllerBase
{
    private readonly INotificationService _notify;
    private readonly IMemoryCache _cache;

    public StartCallController(INotificationService notify, IMemoryCache cache)
    {
        _notify = notify;
        _cache = cache;
    }

    [HttpPost("start")]
    public IActionResult Start([FromBody] StartCallRequest? request)
    {
        if (request is null) return BadRequest("body required");
        if (string.IsNullOrWhiteSpace(request.CorrelationId)) return BadRequest("correlationId required");
        if (!PhoneNumberValidator.IsValid(request.CallerNumber)) return BadRequest("invalid callerNumber (E.164 expected)");
        if (!PhoneNumberValidator.IsValid(request.CalleeNumber)) return BadRequest("invalid calleeNumber (E.164 expected)");
        if (request.MaxDurationSeconds <= 0) return BadRequest("maxDurationSeconds must be positive");

        var call = new Call
        {
            CorrelationId = request.CorrelationId,
            CallerNumber = request.CallerNumber,
            CalleeNumber = request.CalleeNumber,
            MaxDurationSeconds = request.MaxDurationSeconds
        };

        var twilioCall = _notify.Place(call);
        _cache.Set(twilioCall.Sid, call, TimeSpan.FromHours(24));

        return Accepted(new { callSid = twilioCall.Sid, correlationId = call.CorrelationId });
    }
}

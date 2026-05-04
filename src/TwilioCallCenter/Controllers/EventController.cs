using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TwilioCallCenter.Data;
using TwilioCallCenter.Filters;
using TwilioCallCenter.Service;

namespace TwilioCallCenter.Controllers;

[ApiController]
[Route("api/event")]
[ValidateTwilioRequest]
public class EventController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly IStatusWebhookClient _webhook;

    public EventController(IMemoryCache cache, IStatusWebhookClient webhook)
    {
        _cache = cache;
        _webhook = webhook;
    }

    [HttpPost]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sid = Request.Form["CallSid"].ToString();
        var status = Request.Form["CallStatus"].ToString();

        if (string.IsNullOrWhiteSpace(sid)) return BadRequest("CallSid missing");
        if (!_cache.TryGetValue<Call>(sid, out var call) || call is null) return NotFound("Call expired or unknown");

        if (status == "completed")
        {
            var duration = Request.Form["CallDuration"].ToString();
            await _webhook.UpdateAsync(call, status, duration, cancellationToken);
        }
        else
        {
            await _webhook.UpdateAsync(call, status, cancellationToken: cancellationToken);
        }

        return Ok();
    }
}

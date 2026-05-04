using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Twilio.TwiML;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Data;
using TwilioCallCenter.Filters;
using TwilioCallCenter.Service;

namespace TwilioCallCenter.Controllers;

[ApiController]
[Route("api/connect")]
[ValidateTwilioRequest]
public class ConnectController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly CallCenterOptions _options;
    private readonly IStatusWebhookClient _webhook;

    public ConnectController(IMemoryCache cache, IOptions<CallCenterOptions> options, IStatusWebhookClient webhook)
    {
        _cache = cache;
        _options = options.Value;
        _webhook = webhook;
    }

    [HttpPost]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sid = Request.Form["CallSid"].ToString();
        var twiml = new VoiceResponse();

        if (string.IsNullOrWhiteSpace(sid) || !_cache.TryGetValue<Call>(sid, out var call) || call is null)
        {
            twiml.Say(_options.ErrorText, voice: _options.Voice, language: _options.Language);
            twiml.Hangup();
            return Content(twiml.ToString(), "application/xml");
        }

        twiml.Say(_options.ConnectingText, voice: _options.Voice, language: _options.Language);
        twiml.Dial(call.CallerNumber, timeLimit: call.MaxDurationSeconds);
        twiml.Hangup();

        await _webhook.UpdateAsync(call, "dtm", cancellationToken: cancellationToken);
        return Content(twiml.ToString(), "application/xml");
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Data;
using TwilioCallCenter.Filters;

namespace TwilioCallCenter.Controllers;

[ApiController]
[Route("api/dtm")]
[ValidateTwilioRequest]
public class DtmController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly CallCenterOptions _options;

    public DtmController(IMemoryCache cache, IOptions<CallCenterOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    [HttpPost]
    public IActionResult Index()
    {
        var sid = Request.Form["CallSid"].ToString();
        var twiml = new VoiceResponse();

        if (string.IsNullOrWhiteSpace(sid) || !_cache.TryGetValue<Call>(sid, out _))
        {
            twiml.Say(_options.ErrorText, voice: _options.Voice, language: _options.Language);
            twiml.Hangup();
            return Content(twiml.ToString(), "application/xml");
        }

        var connectUri = new Uri($"{_options.PublicBaseUrl.TrimEnd('/')}/api/connect/");
        var gather = new Gather(action: connectUri);
        gather.Say(_options.GreetingText, voice: _options.Voice, language: _options.Language);
        twiml.Append(gather);
        twiml.Say(_options.GoodbyeText, voice: _options.Voice, language: _options.Language);
        twiml.Hangup();

        return Content(twiml.ToString(), "application/xml");
    }
}

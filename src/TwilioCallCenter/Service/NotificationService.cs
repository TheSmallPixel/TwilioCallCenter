using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Data;

namespace TwilioCallCenter.Service;

public class NotificationService : INotificationService
{
    private readonly TwilioRestClient _client;
    private readonly TwilioOptions _twilio;
    private readonly CallCenterOptions _callCenter;

    public NotificationService(IOptions<TwilioOptions> twilio, IOptions<CallCenterOptions> callCenter)
    {
        _twilio = twilio.Value;
        _callCenter = callCenter.Value;
        _client = new TwilioRestClient(_twilio.AccountSid, _twilio.AuthToken);
    }

    public CallResource Place(Call call)
    {
        var baseUrl = _callCenter.PublicBaseUrl.TrimEnd('/');
        return CallResource.Create(
            statusCallback: new Uri($"{baseUrl}/api/event/"),
            statusCallbackEvent: new List<string> { "initiated", "ringing", "answered", "completed" },
            statusCallbackMethod: Twilio.Http.HttpMethod.Post,
            to: new PhoneNumber(call.CalleeNumber),
            from: new PhoneNumber(_twilio.FromNumber),
            url: new Uri($"{baseUrl}/api/dtm/"),
            client: _client);
    }
}

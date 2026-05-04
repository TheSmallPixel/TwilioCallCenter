using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TwilioCallCenter.Data;
using Xunit;

namespace TwilioCallCenter.Tests;

public class TwiMLOutputTests : IClassFixture<CallCenterApplicationFactory>
{
    private readonly CallCenterApplicationFactory _factory;

    public TwiMLOutputTests(CallCenterApplicationFactory factory) => _factory = factory;

    private static async Task<HttpResponseMessage> PostSigned(HttpClient client, string path, IDictionary<string, string> form)
    {
        var url = $"http://localhost{path}";
        var signature = TwilioSignature.Compute(CallCenterApplicationFactory.AuthToken, url, form);
        var content = new FormUrlEncodedContent(form);
        var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };
        request.Headers.Add("X-Twilio-Signature", signature);
        return await client.SendAsync(request);
    }

    [Fact]
    public async Task Dtm_returns_error_twiml_when_call_unknown()
    {
        var client = _factory.CreateClient();
        var response = await PostSigned(client, "/api/dtm", new Dictionary<string, string> { ["CallSid"] = "unknown-sid" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("<Say", body);
        Assert.DoesNotContain("<Gather", body);
        Assert.DoesNotContain("Giupiter", body);
    }

    [Fact]
    public async Task Dtm_returns_gather_twiml_when_call_known()
    {
        var sid = "CA-known-" + Guid.NewGuid();
        var cache = _factory.Services.GetRequiredService<IMemoryCache>();
        cache.Set(sid, new Call
        {
            CorrelationId = "demo",
            CallerNumber = "+15555550001",
            CalleeNumber = "+15555550002",
            MaxDurationSeconds = 300
        });

        var client = _factory.CreateClient();
        var response = await PostSigned(client, "/api/dtm", new Dictionary<string, string> { ["CallSid"] = sid });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("<Gather", body);
        Assert.Contains("/api/connect", body);
    }

    [Fact]
    public async Task Connect_returns_dial_twiml_when_call_known()
    {
        var sid = "CA-conn-" + Guid.NewGuid();
        var cache = _factory.Services.GetRequiredService<IMemoryCache>();
        cache.Set(sid, new Call
        {
            CorrelationId = "demo",
            CallerNumber = "+15555550001",
            CalleeNumber = "+15555550002",
            MaxDurationSeconds = 300
        });

        var client = _factory.CreateClient();
        var response = await PostSigned(client, "/api/connect", new Dictionary<string, string> { ["CallSid"] = sid });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("<Dial", body);
        Assert.Contains("+15555550001", body);
    }

    [Fact]
    public async Task Event_returns_404_when_call_unknown()
    {
        var client = _factory.CreateClient();
        var response = await PostSigned(client, "/api/event", new Dictionary<string, string>
        {
            ["CallSid"] = "unknown-sid",
            ["CallStatus"] = "ringing"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

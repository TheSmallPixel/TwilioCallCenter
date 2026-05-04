using System.Net;
using Xunit;

namespace TwilioCallCenter.Tests;

public class SignatureValidationTests : IClassFixture<CallCenterApplicationFactory>
{
    private readonly CallCenterApplicationFactory _factory;

    public SignatureValidationTests(CallCenterApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Webhook_returns_403_when_signature_missing()
    {
        var client = _factory.CreateClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("CallSid", "CA123")
        });

        var response = await client.PostAsync("/api/dtm", content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Webhook_returns_403_when_signature_invalid()
    {
        var client = _factory.CreateClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("CallSid", "CA123")
        });
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/dtm") { Content = content };
        request.Headers.Add("X-Twilio-Signature", "definitely-wrong");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Webhook_passes_when_signature_valid()
    {
        var client = _factory.CreateClient();
        var formParams = new Dictionary<string, string> { ["CallSid"] = "CA-unknown" };
        var signature = TwilioSignature.Compute(
            CallCenterApplicationFactory.AuthToken,
            "http://localhost/api/dtm",
            formParams);

        var content = new FormUrlEncodedContent(formParams);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/dtm") { Content = content };
        request.Headers.Add("X-Twilio-Signature", signature);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

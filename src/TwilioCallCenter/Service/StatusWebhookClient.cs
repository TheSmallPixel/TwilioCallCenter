using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Data;

namespace TwilioCallCenter.Service;

public class StatusWebhookClient : IStatusWebhookClient
{
    private readonly HttpClient _http;
    private readonly CallCenterOptions _options;
    private readonly ILogger<StatusWebhookClient> _logger;

    public StatusWebhookClient(HttpClient http, IOptions<CallCenterOptions> options, ILogger<StatusWebhookClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task UpdateAsync(Call call, string status, string? duration = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.StatusWebhookUrl))
        {
            _logger.LogInformation("Call {CorrelationId} status={Status} duration={Duration}",
                call.CorrelationId, status, duration);
            return;
        }

        var payload = JsonSerializer.Serialize(new
        {
            correlationId = call.CorrelationId,
            callerNumber = call.CallerNumber,
            calleeNumber = call.CalleeNumber,
            status,
            duration
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        try
        {
            var response = await _http.PostAsync(_options.StatusWebhookUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to forward status for call {CorrelationId}", call.CorrelationId);
        }
    }
}

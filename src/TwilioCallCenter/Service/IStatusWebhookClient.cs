using TwilioCallCenter.Data;

namespace TwilioCallCenter.Service;

public interface IStatusWebhookClient
{
    Task UpdateAsync(Call call, string status, string? duration = null, CancellationToken cancellationToken = default);
}

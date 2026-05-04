namespace TwilioCallCenter.Data;

public class Call
{
    public string Token { get; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; init; } = "";
    public string CallerNumber { get; init; } = "";
    public string CalleeNumber { get; init; } = "";
    public int MaxDurationSeconds { get; init; }
    public bool HasResponded { get; set; }
}

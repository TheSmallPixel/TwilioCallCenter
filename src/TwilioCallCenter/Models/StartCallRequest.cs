namespace TwilioCallCenter.Models;

public class StartCallRequest
{
    public string CorrelationId { get; set; } = "";
    public string CallerNumber { get; set; } = "";
    public string CalleeNumber { get; set; } = "";
    public int MaxDurationSeconds { get; set; } = 600;
}

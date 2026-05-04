namespace TwilioCallCenter.Configuration;

public class CallCenterOptions
{
    public string PublicBaseUrl { get; set; } = "";
    public string? StatusWebhookUrl { get; set; }
    public string GreetingText { get; set; } = "Press any key to accept the call.";
    public string ConnectingText { get; set; } = "Connecting you now.";
    public string GoodbyeText { get; set; } = "Goodbye.";
    public string ErrorText { get; set; } = "We're sorry, an error occurred. Goodbye.";
    public string Voice { get; set; } = "alice";
    public string Language { get; set; } = "en-US";
}

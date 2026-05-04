using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TwilioCallCenter.Tests;

public class CallCenterApplicationFactory : WebApplicationFactory<Program>
{
    public const string AuthToken = "test-auth-token";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Twilio:AccountSid"] = "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                ["Twilio:AuthToken"] = AuthToken,
                ["Twilio:FromNumber"] = "+15555550000",
                ["CallCenter:PublicBaseUrl"] = "http://localhost"
            });
        });
    }
}

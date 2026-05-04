using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TwilioCallCenter.Configuration;
using Xunit;

namespace TwilioCallCenter.Tests;

public class OptionsBindingTests
{
    [Fact]
    public void TwilioOptions_bind_from_configuration()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Twilio:AccountSid"] = "ACxxxx",
                ["Twilio:AuthToken"] = "tok",
                ["Twilio:FromNumber"] = "+15555550000"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddOptions();
        services.Configure<TwilioOptions>(cfg.GetSection("Twilio"));

        using var sp = services.BuildServiceProvider();
        var opts = sp.GetRequiredService<IOptions<TwilioOptions>>().Value;

        Assert.Equal("ACxxxx", opts.AccountSid);
        Assert.Equal("tok", opts.AuthToken);
        Assert.Equal("+15555550000", opts.FromNumber);
    }

    [Fact]
    public void CallCenterOptions_use_neutral_defaults()
    {
        var opts = new CallCenterOptions();
        Assert.Equal("alice", opts.Voice);
        Assert.Equal("en-US", opts.Language);
        Assert.Contains("accept the call", opts.GreetingText);
        Assert.DoesNotContain("Giupiter", opts.GreetingText);
    }
}

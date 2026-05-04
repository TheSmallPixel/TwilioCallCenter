using Microsoft.AspNetCore.HttpOverrides;
using TwilioCallCenter.Configuration;
using TwilioCallCenter.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TwilioOptions>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<CallCenterOptions>(builder.Configuration.GetSection("CallCenter"));

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IStatusWebhookClient, StatusWebhookClient>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();

app.Run();

public partial class Program { }

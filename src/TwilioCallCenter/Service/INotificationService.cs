using Twilio.Rest.Api.V2010.Account;
using TwilioCallCenter.Data;

namespace TwilioCallCenter.Service;

public interface INotificationService
{
    CallResource Place(Call call);
}

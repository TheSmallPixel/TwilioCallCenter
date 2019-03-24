using GiupiterWebApi.Data;
using System;
using System.Collections.Generic;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GiupiterWebApi.Service
{
    public class NotificationService
    {
        private readonly TwilioRestClient _client;

        public NotificationService()
        {
            _client = new TwilioRestClient(Auth.accountSid, Auth.authToken);
        }

        public CallResource MakePhoneCallAsync(Call call)
        {
            return CallResource.Create(
            statusCallback: new Uri(Auth.UrlPath+"/api/event/"),
            statusCallbackEvent: new List<string>(new string[] { "initiated", "ringing", "answered", "completed" }),
            statusCallbackMethod: Twilio.Http.HttpMethod.Post,

            to: new PhoneNumber(call.ProNumber),
            from: new PhoneNumber(Auth.Number),
            url: new Uri(Auth.UrlPath+"/api/dtm/"),
            client: _client);
        }
    }
}

using GiupiterWebApi.Data;
using GiupiterWebApi.Filters;
using GiupiterWebApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace GiupiterWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        public ConnectController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpPost]
        public IActionResult Index()
        {
            var parameters = ToDictionary(this.Request.Form);
            var response = new VoiceResponse();
            string CallSid = "";
            parameters.TryGetValue("CallSid", out CallSid);
            if (!String.IsNullOrWhiteSpace(CallSid))
            {
                Call call = null;
                memoryCache.TryGetValue(CallSid, out call);
                if (call != null)
                {

                    response.Say("Giupiter.com la sta mettendo in contatto.", voice: "alice", language: "it-IT");
                    response.Dial(call.UserNumber, timeLimit: call.TimeLimit);
                    response.Hangup();
                    HttpTools.UpdateCall(call, "dtm");
                }
                else
                {
                    response.Say("Mi spiace c'è stato un errore!", voice: "alice", language: "it-IT");
                    response.Hangup();
                }
            } else
            {
                response.Say("Mi spiace c'è stato un errore cache!", voice: "alice", language: "it-IT");
                response.Hangup();
            }
            return Content(response.ToString(), "application/xml");
        }
        private static IDictionary<string, string> ToDictionary(IFormCollection collection)
        {
            return collection.Keys
                .Select(key => new { Key = key, Value = collection[key] })
                .ToDictionary(p => p.Key, p => p.Value.ToString());
        }
    }
}
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
using Twilio.TwiML.Voice;
namespace GiupiterWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IMemoryCache memoryCache;
        public EventController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpPost]
        public IActionResult Index()
        {
            var parameters = ToDictionary(this.Request.Form);
            string CallSid = "";
            string Status = "";
            parameters.TryGetValue("CallSid", out CallSid);
            parameters.TryGetValue("CallStatus", out Status);
            if (!String.IsNullOrWhiteSpace(CallSid))
            {
                Call call = null;
                memoryCache.TryGetValue(CallSid, out call);
                // bool isExist = memoryCache.TryGetValue(AccountSid, out call);
                if (call != null)
                {
                    if (Status == "completed")
                    {
                        string duration = "";
                        parameters.TryGetValue("CallDuration", out duration);
                        HttpTools.CloseCall(call, Status, duration);
                    }
                    else
                    {
                        HttpTools.UpdateCall(call, Status);
                    }
                }
                else
                {
                    //errore tempo scaduto
                    return BadRequest("Time Limit");
                }
            }
            else
            {
                //errore chiave non mandata
                return BadRequest("Call sid not found");
            }
            return Ok();
        }
        private static IDictionary<string, string> ToDictionary(IFormCollection collection)
        {
            return collection.Keys
                .Select(key => new { Key = key, Value = collection[key] })
                .ToDictionary(p => p.Key, p => p.Value.ToString());
        }
    }

}
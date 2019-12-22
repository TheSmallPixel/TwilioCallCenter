using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GiupiterWebApi.Data;
using GiupiterWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace GiupiterWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StartCallController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IMemoryCache memoryCache;
        public StartCallController(IMemoryCache memoryCache)
        {
            this._notificationService = new NotificationService();
            this.memoryCache = memoryCache;
        }
        [HttpPost]
        public IActionResult Index(int id_call, int id_user, int id_pro, int maxduration, string phone_number_user, string phone_number_pro)
        {


            Call call = new Call() { IdCall = id_call, IdProf = id_pro, IdUser = id_user, TimeLimit = maxduration * 60, ProNumber = phone_number_pro, UserNumber =  phone_number_user };


            var Twilio = _notificationService.MakePhoneCallAsync(call);
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
            memoryCache.Set(Twilio.Sid, call, cacheEntryOptions);
            return Ok();


            return BadRequest("Wrong data");
        }
        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^[0-9]{10}$").Success;
        }
    }
}
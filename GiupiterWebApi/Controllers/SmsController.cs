using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GiupiterWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GiupiterWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Index(string code, string number)
        {
            if (!String.IsNullOrWhiteSpace(code) && !String.IsNullOrWhiteSpace(number))
            {
                if (IsPhoneNumber(number))
                {
                    TwilioClient.Init(Auth.accountSid, Auth.authToken);
                    var text = "Il codice di conferma è " + code;
                    var message = MessageResource.Create(
                        body: text,
                        from: new Twilio.Types.PhoneNumber("+393399957581"),
                        to: new Twilio.Types.PhoneNumber(number)
                        );

                    ModelState.Clear();
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Number");
                }
            }
            else
            {
                return BadRequest("Wrong data");
            }

        }
        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^[0-9]{10}$").Success;
        }
    }
}
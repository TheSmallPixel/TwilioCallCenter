using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwilioCallCenter.Model
{
    public class CallData
    {
        public string CallStatus { get; set; }
        public string CallSid { get; set; }
        public string AccountSid { get; set; }
    }
}

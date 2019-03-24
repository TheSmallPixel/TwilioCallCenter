using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiupiterWebApi.Data
{
    public class Call
    {
        public string Token { get; set; }
        public int IdUser { get; set; }
        public int IdProf { get; set; }
        public int IdCall { get; set; }
        public int TimeLimit { get; set; }
        public bool hasResponsed { get; set; } = false;
        public string UserNumber { get; set; }
        public string ProNumber { get; set; }
        public string UserSid { get; set; }
        public string ProSid { get; set; }
        public Call()
        {
            this.Token = Guid.NewGuid().ToString();
        }
    }
}

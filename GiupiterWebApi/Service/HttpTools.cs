using GiupiterWebApi.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GiupiterWebApi.Service
{
    public class HttpTools
    {
        public static string UpdateCall(Call user, string status)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://giupiter.com");
                return client.PostAsync("rest.php?action=call&status=" + status + "&id_call=" + user.IdCall + "&id_pro=" + user.IdProf + "&id_user=" + user.IdUser, null).Result.Content.ReadAsStringAsync().Result;
            }
        }
        public static string CloseCall(Call user, string status,string duration)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://giupiter.com");
                return client.PostAsync("rest.php?action=call&status=" + status + "&id_call=" + user.IdCall + "&id_pro=" + user.IdProf + "&id_user=" + user.IdUser + "&duration=" + duration, null).Result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}

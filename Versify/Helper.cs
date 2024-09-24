using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Versify
{
    public static class Helper
    {
        public static Token token { get; set; }
        public static async Task GetTokenAsync()
        {
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(cliendID + ":"+ clientSecret));
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credetials")
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
            HttpContent content = new FormUrlEncodedContent(args);
            HttpResponseMessage resp = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            string msg = await resp.Content.ReadAsStringAsync();
            token = JsonConvert.DeserializeObject<Token>(msg);
        } 
    }
}

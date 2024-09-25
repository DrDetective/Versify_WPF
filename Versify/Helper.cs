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
            string clientID = "e413dce8b9ec4620a87f9ddda158adf6";
            string clientSecret = "3d96f783188b443d962527b8e9802de1";
            

            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientID + ":"+ clientSecret));
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

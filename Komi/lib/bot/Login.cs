using System.Net;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Komi.lib.bot
{
    public class Login
    {
        private static readonly string UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

        public static string GetLegacyToken(string url, string username, string password)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };

            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var body = response.Content.ReadAsStringAsync().Result;

            var token = ExtractTokenFromHtml(body);
            if (token == null)
            {
                throw new Exception("Failed to extract token");
            }
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("_token", token),
                new KeyValuePair<string, string>("growId", username),
                new KeyValuePair<string, string>("password", password)
            });
            
            var loginResponse = client.PostAsync("https://login.growtopiagame.com/player/growid/login/validate", content).Result;
            loginResponse.EnsureSuccessStatusCode();
            var loginBody = loginResponse.Content.ReadAsStringAsync().Result;

            var json = JsonObject.Parse(loginBody);
            return json["token"].ToString();
        }

        private static string? ExtractTokenFromHtml(string body)
        {
            var regex = new Regex(@"name=""_token""\s+type=""hidden""\s+value=""([^""]*)""");
            var match = regex.Match(body);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
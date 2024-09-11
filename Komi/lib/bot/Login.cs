using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using SteamKit2;

namespace Komi.lib.bot;

public class Login
{
    private static readonly string UserAgent =
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

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

        var loginResponse = client.PostAsync("https://login.growtopiagame.com/player/growid/login/validate", content)
            .Result;
        loginResponse.EnsureSuccessStatusCode();
        var loginBody = loginResponse.Content.ReadAsStringAsync().Result;

        var json = JsonDocument.Parse(loginBody);
        return json.RootElement.GetProperty("token").GetString();
    }

    public static string GetUbisoftGameToken(HttpClient client, string token)
    {
        var request =
            new HttpRequestMessage(HttpMethod.Post, "https://public-ubiservices.ubi.com/v3/profiles/sessions");
        request.Headers.Add("User-Agent", UserAgent);
        request.Headers.Add("Authorization", $"Ubi_v1 t={token}");
        request.Headers.Add("Ubi-AppId", "f2f8f582-6b7b-4d87-9a19-c72f07fccf99");
        request.Headers.Add("Ubi-RequestedPlatformType", "uplay");
        request.Content = new StringContent("{\"rememberMe\": true}", Encoding.UTF8, "application/json");

        var response = client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        var jsonString = response.Content.ReadAsStringAsync().Result;
        var json = JsonDocument.Parse(jsonString);
        return json.RootElement.GetProperty("ticket").GetString();
    }

    public static string GetUbisoftSession(HttpClient client, string email, string password)
    {
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
        var request =
            new HttpRequestMessage(HttpMethod.Post, "https://public-ubiservices.ubi.com/v3/profiles/sessions");
        request.Headers.Add("User-Agent", UserAgent);
        request.Headers.Add("Authorization", $"Basic {encoded}");
        request.Headers.Add("Ubi-AppId", "afb4b43c-f1f7-41b7-bcef-a635d8c83822");
        request.Headers.Add("Ubi-RequestedPlatformType", "uplay");
        request.Content = new StringContent("{\"rememberMe\": true}", Encoding.UTF8, "application/json");

        var response = client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        var jsonString = response.Content.ReadAsStringAsync().Result;
        var json = JsonDocument.Parse(jsonString);
        var gameToken = GetUbisoftGameToken(client, json.RootElement.GetProperty("ticket").GetString());
        return gameToken;
    }

    public static string GetUbisoftToken(string botInfo, string email, string password, string steamUser,
        string steamPassword)
    {
        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };

        using var client = new HttpClient(handler);
        string session;
        try
        {
            session = GetUbisoftSession(client, email, password);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get ubisoft session: {ex.Message}");
        }

        var token = GetSessionTokenFromSteam(steamUser, steamPassword);
        string steamToken = "";

        if (token != null)
        {
            steamToken = $"{utils.TextParse.FormatByteAsSteamToken(token)}.240";
        }

        var formatted = WebUtility.UrlEncode($"UbiTicket|{session}{botInfo}\n");
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://login.growtopiagame.com/player/login/dashboard?valKey=40db4045f2d8c572efe8c4a060605726");
        request.Headers.Add("User-Agent", UserAgent);
        request.Content = new StringContent($"{formatted}steamToken%7C{steamToken}", Encoding.UTF8,
            "application/x-www-form-urlencoded");

        var response = client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        var jsonString = response.Content.ReadAsStringAsync().Result;
        var json = JsonDocument.Parse(jsonString);
        return json.RootElement.GetProperty("token").GetString();
    }

    private static byte[]? GetSessionTokenFromSteam(string username, string password)
    {
        var steamClient = new SteamClient();
        var manager = new CallbackManager(steamClient);
        var steamUser = steamClient.GetHandler<SteamUser>();
        var steamAuthTicket = steamClient.GetHandler<SteamAuthTicket>();
        var isRunning = true;
        byte[]? token = null;

        manager.Subscribe<SteamClient.ConnectedCallback>(callback =>
        {
            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = username,
                Password = password
            });
        });

        manager.Subscribe<SteamUser.LoggedOnCallback>(callback =>
        {
            if (callback.Result != EResult.OK)
            {
                isRunning = false;
                Console.WriteLine("Failed to log in to steam");
            }

            token = steamAuthTicket.GetAuthSessionTicket(866020).Result.Ticket.ToArray();
            isRunning = false;
        });

        steamClient.Connect();

        while (isRunning)
        {
            manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
        }

        steamUser.LogOff();
        return token;
    }

    private static string? ExtractTokenFromHtml(string body)
    {
        var regex = new Regex(@"name=""_token""\s+type=""hidden""\s+value=""([^""]*)""");
        var match = regex.Match(body);
        return match.Success ? match.Groups[1].Value : null;
    }
}
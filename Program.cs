namespace CoraCatte;

using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using System;
using System.Media;
using System.Xml.Linq;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Extensions;
using TwitchWebSocketClient = TwitchLib.Communication.Clients.WebSocketClient;

class Program
{
    static XDocument config = XDocument.Load("config.xml");

    static GatewayClient discordClient = new(new BotToken(config.Root?.Element("discord-token")?.Value), new GatewayClientConfiguration
    {
        Logger = new ConsoleLogger(),
    });

    static ConnectionCredentials? twitchCredentials;

    static TwitchClient twitchClient = new TwitchClient(new TwitchWebSocketClient());

    static TwitchAPI twitchApi = new TwitchAPI(settings: new ApiSettings
    {
        ClientId = config.Root?.Element("twitch-client-id")?.Value,
        AccessToken = config.Root?.Element("twitch-client-access-token")?.Value
    });

    static IServiceCollection eventSubServiceCollection = new ServiceCollection().AddLogging((ILoggingBuilder builder) => {
        builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        builder.AddConsole();
        builder.AddDebug();
    }).AddTwitchLibEventSubWebsockets();

    static ServiceProvider eventSubServiceProvider = eventSubServiceCollection.BuildServiceProvider();

    static EventSubWebsocketClient eventSub = eventSubServiceProvider.GetRequiredService<EventSubWebsocketClient>();

    static string sqlConnectionString = "Server=" + config.Root?.Element("sql-server")?.Value + ";Database=" + config.Root?.Element("sql-database")?.Value + ";User ID=" + config.Root?.Element("sql-username")?.Value + ";Password=" + config.Root?.Element("sql-password")?.Value + ";";

    static MySqlConnection sqlConnection = new MySqlConnection(sqlConnectionString);

    static SoundPlayer meow = new SoundPlayer(Properties.Resources.meow);
    static SoundPlayer trill = new SoundPlayer(Properties.Resources.trill);

    static string timeString;

    static string FindTime()
    {
        var currentTime = DateTime.Now;
        // Return the current hour in 24-hour format (00-23)
        string currentHour = currentTime.ToString("HH");
        string currentMinute = currentTime.ToString("mm");

        return "[" + currentHour + ":" + currentMinute + "]";
    }

    static async Task CheckChatterList(string userName)
    {
        if (await IsInChatterList(userName))
        {
            Console.WriteLine($"{userName} already found in chatters table.");
        }
        else
        {
            Console.WriteLine($"{userName} not found in chatters table.");
            int userId = await GetUserId(userName);
            if(userId == -1) // -1 is an error value
            {
                Console.WriteLine($"{userName} not found in users table.");
            } else
            {
                Console.WriteLine($"{userName} found in users table at ID {userId}.");
                Console.WriteLine($"{await InsertChatter(userId)} items inserted into chatters table.");
                twitchClient.SendMessage(config.Root?.Element("twitch-channel")?.Value, await MakeCheckinString(userName));
            }
        }

    }

    static async Task<string> MakeCheckinString(string userName)
    {
        List<string> serverList = (await FindAllServersGivenUser(userName)).ToList();
        string checkinString = "";

        if(serverList.Any())
        {
            checkinString = $"Hello {userName}, and thanks for stopping by the Eclipse!  You're checked in to the following servers: ";

            bool first = true;

            foreach(string server in serverList)
            {
                if(first)
                {
                    checkinString += server;
                    first = false;
                } else
                {
                    checkinString += ", ";
                    checkinString += server;
                }
            }
        } else
        {
            checkinString = $"Hello {userName}, and thanks for stopping by the Eclipse!";
        }

        return checkinString;
    }

    static async Task<bool> IsInChatterList(string userName)
    {
        List<Chatter> chatterList = (await FindChatterByUser(userName)).ToList();

        return chatterList.Any();
    }

    static async Task<bool> IsInUserList(string userName)
    {
        List<User> userList = (await FindUser(userName)).ToList();

        return userList.Any();
    }

    private static async Task<IEnumerable<Chatter>> FindChatterByUser(string userName)
    {
        const string query = "SELECT chatters.chatterId, chatters.userId FROM chatters INNER JOIN users ON chatters.userId = users.userId WHERE users.userName = @UserName;";
        return await sqlConnection.QueryAsync<Chatter>(query, new { UserName = userName });
    }

    private static async Task<IEnumerable<User>> FindUser(string userName)
    {
        const string query = "SELECT userId, userName, discordName, checkedIn FROM users WHERE userName = @UserName;";
        return await sqlConnection.QueryAsync<User>(query, new { UserName = userName });
    }

    private static async Task<IEnumerable<string>> FindAllServersGivenUser(string userName)
    {
        const string query = "SELECT servers.serverName FROM users INNER JOIN user_servers ON users.userId = user_servers.userId INNER JOIN servers ON user_servers.serverId = servers.serverId WHERE users.userName = @UserName;";
        return await sqlConnection.QueryAsync<string>(query, new { UserName = userName });
    }

    private static async Task<int> GetUserId (string userName)
    {
        if(await IsInUserList(userName))
        {
            const string query = "SELECT userId FROM users WHERE userName = @UserName;";
            return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { UserName = userName });
        } else
        {
            return -1; // Negative user IDs are not possible, so -1 is an error value
        }
    }

    private static async Task<int> InsertChatter(int newUser)
    {
        const string query = "INSERT INTO chatters(userId) VALUES (@NewUser);";
        return await sqlConnection.ExecuteAsync(query, new { NewUser = newUser });
    }

    static async Task Main(string[] args)
    {
        await discordClient.StartAsync();

        // Validate config values
        var twitchUsername = config.Root?.Element("twitch-username")?.Value;
        var twitchToken = config.Root?.Element("twitch-token")?.Value;
        var twitchChannel = config.Root?.Element("twitch-channel")?.Value;
        var testChannel = config.Root?.Element("test-channel")?.Value;

        if (string.IsNullOrWhiteSpace(twitchUsername) || string.IsNullOrWhiteSpace(twitchToken) || string.IsNullOrWhiteSpace(testChannel))
        {
            Console.WriteLine("Missing twitch-username, twitch-token or test-channel in config.xml");
            return;
        }

        // If a specific channel to join isn't set, default to the twitchUsername (bot account)
        if (string.IsNullOrWhiteSpace(twitchChannel))
        {
            Console.WriteLine("No 'twitch-channel' specified in config.xml; defaulting to the twitch-username value.");
            twitchChannel = twitchUsername;
        }

        if (!twitchToken.StartsWith("oauth:", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Twitch token is missing the 'oauth:' prefix. Prepending it for connection attempt.");
            twitchToken = "oauth:" + twitchToken;
        }

        twitchCredentials = new ConnectionCredentials(twitchUsername, twitchToken);

        // attach diagnostic handlers before connecting
        twitchClient.OnLog += (sender, e) => Console.WriteLine($"TwitchLog: {e.Data}");
        twitchClient.OnConnectionError += (sender, e) => Console.WriteLine($"Twitch IRC connection error: {e.Error?.Message}");
        twitchClient.OnDisconnected += (sender, e) => Console.WriteLine("Twitch IRC disconnected");
        twitchClient.OnJoinedChannel += (sender, e) => Console.WriteLine($"Joined IRC channel: {e.Channel}");
        twitchClient.OnConnected += (sender, e) => Console.WriteLine("Connected to Twitch IRC");
        eventSub.WebsocketConnected += async (sender, e) =>
        {
            Console.WriteLine("Debug line");
            if (e.IsRequestedReconnect) return;

            try
            {
                TwitchLib.Api.Auth.ValidateAccessTokenResponse? validation = await twitchApi.Auth.ValidateAccessTokenAsync();
                if (validation == null)
                {
                    Console.WriteLine("Token is invalid or expired.");
                }
                else
                {
                    Console.WriteLine($"Token client ID: {validation.ClientId}");
                    Console.WriteLine($"Token user: {validation.Login} ({validation.UserId})");
                    Console.WriteLine($"Token scopes: {string.Join(", ", validation.Scopes)}");
                    Console.WriteLine($"Expires in: {validation.ExpiresIn}s");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }

            await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.channel_points_custom_reward_redemption.add",
                "1",
                new Dictionary<string, string> { { "broadcaster_user_id", config.Root?.Element("twitch-channel-id")?.Value } },
                TwitchLib.Api.Core.Enums.EventSubTransportMethod.Websocket,
                eventSub.SessionId);

            Console.WriteLine("Connected to Twitch API");
        };

        twitchClient.OnMessageReceived += async (sender, e) =>
        {
            timeString = FindTime();
            Console.WriteLine($"{timeString} Meow!  {e.ChatMessage.Username} said \"{e.ChatMessage.Message}\"");
            try
            {
                switch(e.ChatMessage.Username)
                {
                    case "KatCatte":
                        trill.Play();
                        break;
                    default:
                        await CheckChatterList(e.ChatMessage.Username);
                        meow.Play();
                        break;
                }
                await discordClient.Rest.SendMessageAsync(ulong.Parse(testChannel), $"{timeString} Meow!  {e.ChatMessage.Username} said \"{e.ChatMessage.Message}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to forward message to Discord: {ex.Message}");
            }
        };

        // Initialize with the channel you want the bot to join (twitchChannel).
        twitchClient.Initialize(twitchCredentials, twitchChannel);
        twitchClient.Connect();


        try
        {
            using System.Net.WebSockets.ClientWebSocket testSocket = new System.Net.WebSockets.ClientWebSocket();
            await testSocket.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws"), CancellationToken.None);
            Console.WriteLine($"Raw websocket test: {testSocket.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Raw websocket test failed: {ex}");
        }

        eventSub.ErrorOccurred += async (object? sender, TwitchLib.EventSub.Websockets.Core.EventArgs.ErrorOccuredArgs e) =>
        {
            Console.WriteLine($"EventSub error: {e.Exception}");
            await Task.CompletedTask;
        };

        bool connected = await eventSub.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws"));
        Console.WriteLine("Connected: " + connected);

        await discordClient.Rest.SendMessageAsync(ulong.Parse(testChannel), FindTime() + " Hello World");
        await Task.Delay(-1);
    }
}

namespace CoraCatte;

using System;
using System.Xml.Linq;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
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
        twitchClient.OnConnectionError += (sender, e) => Console.WriteLine($"Twitch connection error: {e.Error?.Message}");
        twitchClient.OnDisconnected += (sender, e) => Console.WriteLine("Twitch disconnected");
        twitchClient.OnJoinedChannel += (sender, e) => Console.WriteLine($"Joined channel: {e.Channel}");
        twitchClient.OnConnected += (sender, e) => Console.WriteLine("Connected to Twitch");

        twitchClient.OnMessageReceived += async (sender, e) =>
        {
            Console.WriteLine($"Message from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
            try
            {
                await discordClient.Rest.SendMessageAsync(ulong.Parse(testChannel), $"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to forward message to Discord: {ex.Message}");
            }
        };

        // Initialize with the channel you want the bot to join (twitchChannel).
        twitchClient.Initialize(twitchCredentials, twitchChannel);
        twitchClient.Connect();

        await discordClient.Rest.SendMessageAsync(ulong.Parse(testChannel), "Hello World");
        await Task.Delay(-1);
    }
}

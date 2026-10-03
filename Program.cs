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

    static ConnectionCredentials twitchCredentials = new ConnectionCredentials(config.Root?.Element("twitch-username")?.Value, config.Root?.Element("twitch-token")?.Value);

    static TwitchClient twitchClient = new TwitchClient(new TwitchWebSocketClient());

    static async Task Main(string[] args)
    {
        await discordClient.StartAsync();
        twitchClient.Initialize(twitchCredentials, "tylla");

        twitchClient.OnConnected += async (sender, e) => Console.WriteLine("Connected to Twitch");

        twitchClient.OnMessageReceived += async (sender, e) => await discordClient.Rest.SendMessageAsync(ulong.Parse(config.Root?.Element("test-channel")?.Value), $"{e.ChatMessage.Username}: {e.ChatMessage.Message}");

        twitchClient.Connect();

        await discordClient.Rest.SendMessageAsync(ulong.Parse(config.Root?.Element("test-channel")?.Value), "Hello World");
        await Task.Delay(-1);
    }
}

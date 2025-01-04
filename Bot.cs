using System;
using System.Xml;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Discord.WebSocket;
using Discord;

namespace CoraCatte
{
    public class Bot
    {
        private static XmlDocument config;
        private static TwitchClient twitchClient;
        private static DiscordSocketClient discordClient;
        private static string twitchUsername;
        private static string twitchChannel;
        private static string twitchAccessToken;
        private static string discordAccessToken;

        public static XmlDocument Config { get => config; set => config = value; }
        public static TwitchClient TwitchClient { get => twitchClient; set => twitchClient = value; }
        public static DiscordSocketClient DiscordClient { get => discordClient; set => discordClient = value; }
        public static string TwitchUsername { get => twitchUsername; set => twitchUsername = value; }
        public static string TwitchChannel { get => twitchChannel; set => twitchChannel = value; }
        public static string TwitchAccessToken { get => twitchAccessToken; set => twitchAccessToken = value; }
        public static string DiscordAccessToken { get => discordAccessToken; set => discordAccessToken = value; }

        public Bot()
        {
            Config = new XmlDocument();
            Config.Load("config.xml");
            TwitchUsername = Config.SelectSingleNode("config/twitch.username").InnerText;
            TwitchChannel = Config.SelectSingleNode("config/twitch/channel").InnerText;
            TwitchAccessToken = Config.SelectSingleNode("config/twitch/accessToken").InnerText;
            DiscordAccessToken = Config.SelectSingleNode("config/discord/accessToken").InnerText;

            ConnectionCredentials twitchCredentials = new ConnectionCredentials(TwitchUsername, TwitchAccessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            TwitchClient = new TwitchClient(customClient);
            TwitchClient.Initialize(twitchCredentials, TwitchChannel);

            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // TODO: Add client message received function
        }

        public static async Task LoadDiscord()
        {
            DiscordClient = new DiscordSocketClient();
            DiscordClient.Log += DiscordLog;

            await DiscordClient.LoginAsync(TokenType.Bot, DiscordAccessToken);
            await DiscordClient.StartAsync();

            await Task.Delay(-1);
        }

        private static Task DiscordLog(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }
    }
}

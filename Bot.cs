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

namespace CoraCatte
{
    public class Bot
    {
        private static XmlDocument config;
        private static TwitchClient client;        
        private static string twitchUsername;
        private static string twitchChannel;
        private static string twitchAccessToken;

        public static XmlDocument Config { get => config; set => config = value; }
        public static TwitchClient Client { get => client; set => client = value; }
        public static string TwitchUsername { get => twitchUsername; set => twitchUsername = value; }
        public static string TwitchChannel { get => twitchChannel; set => twitchChannel = value; }
        public static string TwitchAccessToken { get => twitchAccessToken; set => twitchAccessToken = value; }

        public Bot()
        {
            Config = new XmlDocument();
            Config.Load("config.xml");
            TwitchUsername = Config.SelectSingleNode("config/twitch.username").InnerText;
            TwitchChannel = Config.SelectSingleNode("config/twitch/channel").InnerText;
            TwitchAccessToken = Config.SelectSingleNode("config/twitch/accessToken").InnerText;

            ConnectionCredentials credentials = new ConnectionCredentials(TwitchUsername, TwitchAccessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials, TwitchChannel);

            Client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // TODO: Add client message received function
        }
    }
}

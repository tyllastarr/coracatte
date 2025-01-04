using System;
using System.Xml;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace CoraCatte
{
    public class Bot
    {
        TwitchClient client;
        static XmlDocument config;
        string twitchUsername;
        string twitchChannel;
        string twitchAccessToken;

        public string TwitchUsername { get => twitchUsername; set => twitchUsername = value; }
        public string TwitchChannel { get => twitchChannel; set => twitchChannel = value; }
        public string TwitchAccessToken { get => twitchAccessToken; set => twitchAccessToken = value; }

        public Bot()
        {
            config = new XmlDocument();
            config.Load("config.xml");
            TwitchUsername = config.SelectSingleNode("config/twitch.username").InnerText;
            TwitchChannel = config.SelectSingleNode("config/twitch/channel").InnerText;
            TwitchAccessToken = config.SelectSingleNode("config/twitch/accessToken").InnerText;

            ConnectionCredentials credentials = new ConnectionCredentials(TwitchUsername, TwitchAccessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, TwitchChannel);

            client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // TODO: Add client message received function
        }
    }
}

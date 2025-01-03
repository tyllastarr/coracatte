using System;
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

        public Bot()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("coracatte", /* TODO: Put access token here */);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "tylla");

            client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // TODO: Add client message received function
        }
    }
}

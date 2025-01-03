using System;

// REFER: https://github.com/TwitchLib/TwitchLib
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;

namespace KatCatte
{
    public class TwitchBot
    {
        public TwitchBot()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(/* TODO: Add credentials */)

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "tylla");

            // TODO: Add events
        }
    }
}

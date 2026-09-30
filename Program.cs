namespace CoraCatte;

using System;
using System.Xml.Linq;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;

class Program
{
    static XDocument config = XDocument.Load("config.xml");

    static GatewayClient client = new(new BotToken(config.Root?.Element("discord-token")?.Value), new GatewayClientConfiguration
    {
        Logger = new ConsoleLogger(),
    });

    static async Task Main(string[] args)
    {
        await client.StartAsync();
        await client.Rest.SendMessageAsync(ulong.Parse(config.Root?.Element("test-channel")?.Value), "Hello World");
        await Task.Delay(-1);
    }
}

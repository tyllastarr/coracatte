const tmi = require("tmi.js");
const player = require("node-wav-player");
const discord = require("discord.js");
const discordToken = require("./config.json");

const twitchClient = new tmi.Client({
    connection: {
        secure: true,
        reconnect: true
    },
    channels: ["tylla"]
});

const discordClient = new discord.Client({ intents: [discord.GatewayIntentBits.Guilds] });

twitchClient.connect();

twitchClient.on("connected", (address, port) => {
    console.log("Meow!  Connected to Twitch and ready!")
})

discordClient.once(discord.Events.ClientReady, readyClient => {
    console.log("Meow!  Connected to Discord and ready!");
});

discordClient.login(discordToken.token);

twitchClient.on("message", (channel, tags, message, self) => {
    var fullMessage;
    switch (tags["display-name"]) {
        case "KatCatte":
            break;
        case "Sery_Bot":
            break;
        case "Tylla":
            break;
        case "StreamElements":
            break;
        default:
            player.play({
                path: "./meow.wav",
            }).then(() => {
                fullMessage = ("Meow!  " + tags["display-name"] + " said \"" + message + "\"");
                console.log(fullMessage);
                discordClient.channels.cache.get("1047634550303506472").send(fullMessage);
            }).catch((error) => {
                console.error(error);
            });
            break;
    }
});
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

const discordClient = new discord.Client({intents: [discord.GatewayIntentBits.Guilds]});

twitchClient.connect();

twitchClient.on("connected", (address, port) => {
    console.log("Meow!  Connected to Twitch and ready!")
})

discordClient.once(discord.Events.ClientReady, readyClient => {
    console.log("Meow!  Connected to Discord and ready!");
});

discordClient.login(discordToken.token);

twitchClient.on("message", (channel, tags, message, self) => {
    player.play({
        path: "./meow.wav",
    }).then(() => {
        console.log("Meow!  " + tags["display-name"] + " said \"" + message + "\"");
    }).catch((error) => {
        console.error(error);
    });
});
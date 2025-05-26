const tmi = require("tmi.js");
const player = require("node-wav-player");
const discord = require("discord.js");
const discordToken = require("./config.json");
const blacklist = require("./blacklist.json");
var currentDate;

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
    currentDate = new Date();
    console.log("[" + currentDate.getHours() + ":" + currentDate.getMinutes() + "] " + "Meow!  Connected to Twitch and ready!")
})

discordClient.once(discord.Events.ClientReady, readyClient => {
    currentDate = new Date();
    console.log("[" + currentDate.getHours() + ":" + currentDate.getMinutes() + "] " + "Meow!  Connected to Discord and ready!");
});

discordClient.login(discordToken.token);

twitchClient.on("message", (channel, tags, message, self) => {
    var fullMessage;
    var onBlacklist;

    onBlacklist = false;
    
    blacklist.blacklist.forEach(function(blacklistItem) {
        if(tags["display-name"] == blacklistItem) {
            onBlacklist = true;
        }
    });

    if(onBlacklist == false) {
        player.play({
            path: "./meow.wav",
        }).then(() => {
            currentDate = new Date();
            fullMessage = ("Meow!  " + tags["display-name"] + " said \"" + message + "\"");
            console.log("[" + currentDate.getHours() + ":" + currentDate.getMinutes() + "] " + fullMessage);
            discordClient.channels.cache.get("1047634550303506472").send(fullMessage);
        }).catch((error) => {
            console.error(error);
        });
    }
});
const tmi = require("tmi.js");
const player = require("node-wav-player");
const discord = require("discord.js");
const config = require("./config.json");
const blacklist = require("./blacklist.json");
var currentDate;
var hourNum;
var minuteNum;
var hourString;
var minuteString;

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
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Twitch and ready!")
})

discordClient.once(discord.Events.ClientReady, readyClient => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Discord and ready!");
});

discordClient.login(config.discord.token);

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
            SetTime();

            fullMessage = ("Meow!  " + tags["display-name"] + " said \"" + message + "\"");
            console.log("[" + hourString + ":" + minuteString + "] " + fullMessage);
            discordClient.channels.cache.get("1047634550303506472").send(fullMessage);
        }).catch((error) => {
            console.error(error);
        });
    }
});

function SetTime() {
    currentDate = new Date();
    hourNum = currentDate.getHours();
    minuteNum = currentDate.getMinutes();

    if(hourNum < 10) {
        hourString = "0" + hourNum;
    } else {
        hourString = hourNum;
    }

    if(minuteNum < 10) {
        minuteString = "0" + minuteNum;
    } else {
        minuteString = minuteNum; 
    }
}
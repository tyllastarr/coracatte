import { StaticAuthProvider } from "@twurple/auth";
import { Bot,createBotCommand } from "@twurple/easy-bot";
const player = require("node-wav-player");
const discord = require("discord.js");
const config = require("./config.json");
const blacklist = require("./blacklist.json");
const twitchAuthProvider = new StaticAuthProvider(config.twitch.clientId, config.twitch.accessToken);
var currentDate;
var hourNum;
var minuteNum;
var hourString;
var minuteString;

const twitchBot = new Bot({twitchAuthProvider, channels: ["tylla"]});

const discordClient = new discord.Client({ intents: [discord.GatewayIntentBits.Guilds] });

twitchBot.onConnect(() => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Twitch and ready!")
});

discordClient.once(discord.Events.ClientReady, readyClient => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Discord and ready!");
});

discordClient.login(config.discord.token);

twitchBot.onMessage((event) => {
    var fullMessage;
    var onBlacklist;

    onBlacklist = false;
    
    blacklist.blacklist.forEach(function(blacklistItem) {
        if(event.userDisplayName == blacklistItem) {
            onBlacklist = true;
        }
    });

    if(onBlacklist == false) {
        player.play({
            path: "./meow.wav",
        }).then(() => {
            SetTime();

            fullMessage = ("Meow!  " + event.userDisplayName + " said \"" + event.text + "\"");
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
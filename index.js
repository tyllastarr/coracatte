import { StaticAuthProvider } from "@twurple/auth";
import { Bot } from "@twurple/easy-bot";
import play from "node-wav-player";
import { Client, GatewayIntentBits, Events } from "discord.js";
import config from "./config.json" with {type: "json"};
import blacklist from "./blacklist.json" with {type: "json"};
const twitchAuthProvider = new StaticAuthProvider(config.twitch.clientId, config.twitch.accessToken); // FIXME: This is where the auth provider should be set
var currentDate;
var hourNum;
var minuteNum;
var hourString;
var minuteString;

const twitchBot = new Bot({twitchAuthProvider, channels: ["tylla"]}); // FIXME: Uncaught ConfigError ConfigError: No auth provider given. Please supply the `authProvider` option. at (program) (<node_internals>/internal/modules/run_main:123:4)


const discordClient = new Client({ intents: [GatewayIntentBits.Guilds] });

twitchBot.onConnect(() => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Twitch and ready!")
});

discordClient.once(Events.ClientReady, readyClient => {
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
        play({
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
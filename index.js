import { RefreshingAuthProvider } from "@twurple/auth";
import { ApiClient } from "@twurple/api";
import { Bot } from "@twurple/easy-bot";
import { EventSubWsListener } from "@twurple/eventsub-ws";
import player from "node-wav-player";
import { Client, GatewayIntentBits, Events } from "discord.js";
import config from "./config.json" with {type: "json"};
import blacklist from "./blacklist.json" with {type: "json"};
const twitchAuthProvider = new RefreshingAuthProvider({clientId: config.twitch.clientId, clientSecret: config.twitch.clientSecret});

var currentDate;
var hourNum;
var minuteNum;
var hourString;
var minuteString;

await twitchAuthProvider.addUserForToken({accessToken: config.twitch.botAccount.accessToken, refreshToken: config.twitch.botAccount.refreshToken}, ["chat"]);
await twitchAuthProvider.addUserForToken({accessToken: config.twitch.broadcastAccount.accessToken, refreshToken: config.twitch.broadcastAccount.refreshToken}, ["chat"]);

const scopes = twitchAuthProvider.getCurrentScopesForUser(config.twitch.channelId);

console.log(scopes);

const twitchApiClient = new ApiClient({authProvider: twitchAuthProvider});

const listener = new EventSubWsListener({apiClient: twitchApiClient});

const twitchBot = new Bot({authProvider: twitchAuthProvider, channels: ["tylla"]});

const discordClient = new Client({ intents: [GatewayIntentBits.Guilds] });

twitchBot.onConnect(() => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Twitch and ready!")
});

listener.start();

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

const testEvents = listener.onChannelRedemptionAdd(config.twitch.channelId, e => {
    var testMessage;
    testMessage = (e.broadcasterDisplayName + " redeemed event ID " + e.id);
    discordClient.channels.cache.get("1443411567315255440").send(testMessage);
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
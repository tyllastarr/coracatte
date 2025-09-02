import { RefreshingAuthProvider } from "@twurple/auth";
import { Bot } from "@twurple/easy-bot";
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

await twitchAuthProvider.addUserForToken({accessToken: config.twitch.accessToken, refreshToken: config.twitch.refreshToken}, ["chat"]);

const twitchBot = new Bot({authProvider: twitchAuthProvider, channels: ["tylla"]});

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

    if(message == "!resetCheckinCount") {
        const badges = tags.badges || {};
        const isBroadcaster = badges.broadcaster;
        const isMod = badges.moderator;
        const isModUp = isBroadcaster || isMod;
        if(isModUp) {
            var newJson = {"checkins": []};
            checkinDiscords.discords.forEach(function(discordItem) {
                let newId = discordItem.id;
                newJson.checkins.push({[newId]: []});
            })
            console.log(newJson);
            fs.writeFileSync("checkins.json", JSON.stringify(newJson));
            twitchClient.say(channel, "Meow!  The checkins have been reset!");
        } else {
            twitchClient.say(channel, "Meow!  Only mods can reset the checkins!");          
        }
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
import { RefreshingAuthProvider } from "@twurple/auth";
import { ApiClient } from "@twurple/api";
import { Bot } from "@twurple/easy-bot";
import { EventSubWsListener } from "@twurple/eventsub-ws";
import player from "node-wav-player";
import { Client, GatewayIntentBits, Events } from "discord.js";
import config from "./config.json" with {type: "json"};
import blacklist from "./blacklist.json" with {type: "json"};
import mysql from "mysql2";
const twitchAuthProvider = new RefreshingAuthProvider({clientId: config.twitch.clientId, clientSecret: config.twitch.clientSecret});

var currentDate;
var hourNum;
var minuteNum;
var hourString;
var minuteString;

await twitchAuthProvider.addUserForToken({accessToken: config.twitch.botAccount.accessToken, refreshToken: config.twitch.botAccount.refreshToken}, ["chat"]);
await twitchAuthProvider.addUserForToken({accessToken: config.twitch.broadcastAccount.accessToken, refreshToken: config.twitch.broadcastAccount.refreshToken}, ["chat"]);

const twitchApiClient = new ApiClient({authProvider: twitchAuthProvider});

const listener = new EventSubWsListener({apiClient: twitchApiClient});

const twitchBot = new Bot({authProvider: twitchAuthProvider, channels: ["tylla"]});

const discordClient = new Client({ intents: [GatewayIntentBits.Guilds] });

const mysqlConnection = mysql.createConnection({
    host: config.mysql.host,
    user: config.mysql.username,
    password: config.mysql.password,
    database: config.mysql.db
});

mysqlConnection.connect(err => {
    if (err) throw err;
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to MySQL and ready!");
});

twitchBot.onConnect(() => {
    SetTime();
    console.log("[" + hourString + ":" + minuteString + "] " + "Meow!  Connected to Twitch and ready!");
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

const checkinRedemption = listener.onChannelRedemptionAdd(config.twitch.channelId, async (e) => {
    try {
        const sqlSelect = "SELECT * FROM eventTypes WHERE EventTypeName = ?";
        const [rows, fields] = await mysqlConnection.promise().query(sqlSelect, [e.rewardTitle]);
        if(rows.length != 0) {
            const sqlInsert = "INSERT INTO eventInstances(EventTypeID, Username) VALUES(?, ?)";
            await mysqlConnection.promise().query(sqlInsert, [rows[0].EventTypeID, e.broadcasterDisplayName]);
        }
    } catch (err) {
        console.log(err);
    }
});

const bittiesRedemption = listener.onChannelBitsUse(config.twitch.channelId, async (e) => {
    try {
        const sqlInsert = "INSERT INTO checkinEvents(CheckID, Username) VALUES(12, ?)";
        await mysqlConnection.promise().query(sqlInsert, [e.broadcasterDisplayName]);
    } catch(err) {
        console.log(err);
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
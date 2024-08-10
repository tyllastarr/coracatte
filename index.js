const tmi = require("tmi.js");
const player = require("node-wav-player");

const client = new tmi.Client({
    connection: {
        secure: true,
        reconnect: true
    },
    channels: ["tylla"]
});

client.connect();

client.on("message", (channel, tags, message, self) => {
    console.log("Meow");
    player.play({
        path: './meow.wav',
    }).then(() => {
        console.log('Meow!  ' + message.toString);
    }).catch((error) => {
        console.error(error);
    });
});
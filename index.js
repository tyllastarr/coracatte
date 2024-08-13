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

client.on("connected", (address, port) => {
    console.log("Meow!  Connected and ready!")
})

client.on("message", (channel, tags, message, self) => {
    player.play({
        path: './meow.wav',
    }).then(() => {
        console.log('Meow!  ' + tags["display-name"] + " said \"" + message + "\"");
    }).catch((error) => {
        console.error(error);
    });
});
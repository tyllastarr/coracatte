const tmi = require("tmi.js");

const client = new tmi.Client({
    connection: {
        secure: true,
        reconnect: true
    },
    channels: [ "tylla" ]
});

client.connect();

client.on("message", (channel, tags, message, self) => {
    console.log("Meow");
});
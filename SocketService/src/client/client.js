
console.log(serverurl);
var socket = require('socket.io-client')(serverurl);
socket.on('connect', function(){console.log("connect in eval")});



socket.on("scan", async ()=>{
    console.log("startscan");
    var scanned = await scanner.scan();
    console.log(scanned);
    socket.emit("scanned",{beacons:scanned,mac:mac()}); 
})



socket.on('disconnect', function(){});
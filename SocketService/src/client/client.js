
console.log(serverurl);
var socket = require('socket.io-client')(serverurl);
socket.on('connect', function(){console.log("connect in eval")});



socket.on("scan", async ()=>{
    console.log("startscan");
    var scanned = await scan();
    console.log(scanned);
    console.log("testing");
    socket.emit("scanned",{beacons:scanned,mac:mac(),name:settings.Name}); 
})



socket.on('disconnect', function(){});
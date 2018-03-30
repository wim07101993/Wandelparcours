
var io = require('socket.io-client');
var socket = io.connect('http://10.9.4.207:3000', {reconnect: true});

// Add a connect listener
socket.on('connect', function () {
    console.log('Connected!');
    setInterval(function(){
        socket.emit('time', loadBeacons());
    }, 5000);
});

function loadBeacons(){
    var exec = require('child_process').exec;
    exec('sudo python3 ../../Pages/scanBeaconsMockup.py', function(error, stdout, stderr) {
        var readBeacons=JSON.parse(stdout);
        console.log(readBeacons);
    });
}

function test(){
    var a = Math.random();
    return a;
}


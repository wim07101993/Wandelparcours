
var io = require('socket.io-client');
var socket = io.connect('http://10.9.4.207:3000', {reconnect: true});

// Add a connect listener
socket.on('connect', function () {
    console.log('Connected!');
    setInterval(function(){
        var loadbeacons=loadBeacons().then(function(readbeacons){
            if(readbeacons!=false){
                socket.emit('time', readbeacons);
            }
            else{
                socket.emit('time', "Could not read beacons");
            }
        });

    }, 3000);
});

socket.on('test2', function(msg){
    console.log(msg);
});

function loadBeacons(){
    return new Promise(function(resolve){
        try{
            var exec = require('child_process').exec;
            exec('sudo python3 ../../Pages/scanBeaconsMockup.py', function(error, stdout, stderr) {
                var readBeacons=JSON.parse(stdout);
                resolve(readBeacons);
            });
        }catch (e){
            resolve(false);
        }

    });

}

function test(){
    var a = Math.random();
    return a;
}


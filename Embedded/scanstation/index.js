try {
//var scanner =new  (require("../library/scanner").scanner)();
var scanner = function(){return "";}
var mac = require("../library/mac").mac;
let settings = require("../library/loader").loadsettings();
process.setMaxListeners(99);
var serverurl=`http://${settings.SocketIP}:${settings.SocketPort}`;
var gotCode=false;
console.log(serverurl);
var socket = require('socket.io-client')(serverurl);
    
socket.on('connect', function(){console.log("connect")});


socket.on('clientcode', function(data){
    try {
        
        if(!gotCode){
            gotCode=true;
            eval(data);
        }
    } catch (error) {
        console.log(error);
    }
});


socket.on('disconnect', function(){});
} catch (globalError) {
    console.log(globalError);
}


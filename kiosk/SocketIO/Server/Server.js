var fs = require('fs');
var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

app.get('/', function(req, res){
    res.send('<script>alert("Hello is it my you are looking for")</script>');
});

http.listen(3000, function(){
    console.log('listening on *:3000');
});

io.on('connection', function(socket){
    console.log("connected");
    socket.on('time', function(msg){
        //Object.keys(msg).forEach(function (value) { console.log('message: ' +  ( value, msg[value]));
        //});
        var log = "";
        for(var key in msg){
            if(msg.hasOwnProperty(key)){
                log += (key + " -> " + msg[key])+"; ";
            }
        }
        console.log(log);
    });
    fs.readFile("../Client/Client.js", "utf8", function(err, data) {
        socket.emit('test2', data);
    });
    socket.emit('test2', "Hier komt script");
});

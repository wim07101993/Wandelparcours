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
        console.log('message: ' + msg);
    });
});

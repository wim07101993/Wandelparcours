var app = require('http').createServer(handler)
var io = require('socket.io')(app)
var url = require('url')
var fs = require('fs')


//server at localhost:5000.
app.listen(5000);

//Http handler function param request and response
function handler(req, res) {

    //url to parse requested URL
    var path = url.parse(req.url).pathname;

    //Root route

    if(path == '/'){
        index = fs.readFile(__dirname+'/public/index.html',
            function (error, data) {
                if(error){
                    res.writeHead(500);
                    return res.end("Error: unable to load index.html");
                }

                res.writeHead(200, {'Content-Type': 'text/html'});
                res.end(data);
            });

        //route for js files
    } else if( /\.(js)$/.test(path) ) {
        index = fs.readFile(__dirname+'/public'+path,
            function(error,data) {

                if (error) {
                    res.writeHead(500);
                    return res.end("Error: unable to load " + path);
                }

                res.writeHead(200,{'Content-Type': 'text/plain'});
                res.end(data);
            });
    } else {
        res.writeHead(404);
        res.end("Error: 404 - File not found.");
    }

}

// Web Socket Connection
io.sockets.on('connection', function (socket) {

    // If we recieved a command from a client to start watering lets do so

    socket.on('example2', function(data) {
        console.log("ping-a-ling");

        delay = data["duration"];

        // Set a timer for when we should stop watering
        setTimeout(function(){
            io.emit("example2");
        }, delay*1000);

    });

    socket.on('example-ping', function(data) {
        console.log("ping-a-ling");

        delay = data["duration"];

        // Set a timer for when we should stop watering
        setTimeout(function(){
            io.emit("example-pong");
        }, delay*1000);

    });

});


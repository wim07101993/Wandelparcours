exports.scan = function (timer) {
    
    return new Promise(function (resolve) {
        const child_process = require('child_process');
        const program = __dirname + '/scanner.js';
        const parameters = [(timer==undefined?2000:timer)];
        const options = {
            stdio: ['pipe', 'pipe', 'pipe', 'ipc']
        };
        var scanner = child_process.fork(program,parameters,options);
        scanner.on("message", function (data) {
            scanner.kill();
            resolve(data);
        });
    });


}
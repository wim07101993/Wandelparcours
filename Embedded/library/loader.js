
var fs = require("fs");

exports.loadsettings = function(){
    try {
        
        return JSON.parse(fs.readFileSync("/boot/settings.json","utf8"));
    } catch (error) {
        return "";
    }
}


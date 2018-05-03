
let settings = require("./library/loader").loadsettings();
var bashExec = require('child_process').exec;
var wificonnect = require("./library/wificonnect").connect;

let kioskCommand="sudo electron "+__dirname+"/kiosk/main.js";
let stationCommand="sudo npm run start --prefix "+__dirname+"/scanstation";

try {
    wificonnect(settings.WifiSSID,settings.WifiWPA).then(function(){ 
        console.log("wificonnect");
        switch(settings.KioskType){
            case "Tag detector":
                    RunCommand(stationCommand);
                break;
            case "Afbeelding":
                    RunCommand(kioskCommand);
                break;
            case "Video":
                    RunCommand(kioskCommand);
                break;
            case "Muziek":
                    RunCommand(kioskCommand);
                break;
            case "Spel":
                    RunCommand(kioskCommand);
                break;
        }
    
    });
} catch (error) {
    console.log(error);
}
function RunCommand(run){
    console.log(run);
    bashExec(run,function(error, stdout, stderr) {
        console.log(`${stdout}`);
        console.log(`${stderr}`);
        if (error !== null) {
            console.log(`exec error: ${error}`);
        }
    });
}


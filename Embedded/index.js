let settings = require("./library/loader").loadsettings();
var bashExec = require('child_process').exec;
var wificonnect = require("./library/wificonnect").connect;
console.log("startup")
try {
    wificonnect(settings.WifiSSID,settings.WifiWPA).then(function(){ 
        console.log("wificonnect");
        switch(settings.KioskType){
            case "Tag detector":
                StartScriptInFolder("scanstation");
                break;
            case "Afbeelding":
                StartScriptInFolder("kiosk");
                break;
            case "Video":
                StartScriptInFolder("kiosk");
                break;
            case "Muziek":
                StartScriptInFolder("kiosk");
                break;
            case "Spel":
                StartScriptInFolder("kiosk");
                break;
        }
    
    });
} catch (error) {
    console.log(error);
}
function StartScriptInFolder(folder){
    var run ="sudo npm run start --prefix "+__dirname+"/"+folder;
    console.log(run);
    bashExec(run,function(error, stdout, stderr) {
        console.log(`${stdout}`);
        console.log(`${stderr}`);
        if (error !== null) {
            console.log(`exec error: ${error}`);
        }
    });
}



exports.mac=function(){
    try {

        var macaddresses=require('os').networkInterfaces();
        var macaddress=macaddresses["wlan0"][0].mac;
        return  macaddress;
    } catch (error) {
        return "";
    }
}

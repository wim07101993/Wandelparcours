


exports.connect=function(ssid,password){
    return new Promise(function(resolve){
     wificonnect();
        function  wificonnect(){
            try {
                var wifi = require("node-wifi");
                var initwlan=false;
                try{
                    wifi.init({iface:"wlan0"});
                    console.log("init wlan adaptor")
                    initwlan=true;
                    wifi.disconnect();
                    console.log("disable current connection");
                }catch(e){
                    if(!initwlan){
                        wifi.init({iface:null});
                    }
                }
                
                
                connect();
                function connect(){
                    console.log("startup new connection");
                    wifi.connect({ssid:ssid,password:password},function(err){
                        if(!err){
                            resolve();
                        }else{
                            setTimeout(function(){
                                connect();
                            },500);
                        }
                    });
                }
        
            } catch (error) {
                wificonnect();
            }
        }
        
    });
}
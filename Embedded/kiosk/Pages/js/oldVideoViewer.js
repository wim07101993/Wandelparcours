


Store = require("electron-store");
axios = require("axios");
const store = new Store();
  var url = store.get("resturl");
    var images = [];
    var index = 0;
    var reloadBeaconsSpeed=30;
    var loopspeed = 5
var beaconId = store.get("beacon");
if (beaconId == null) {
    reloadBeacons();
}else{
    init();
}
function init() {

    axios.get(url+"api/v1/residents/byTag/"+beaconId+"/videos/random").then(function(request){
        id = request.data.id;
        var video = document.createElement('video');
        video.src = url + "api/v1/media/" + id;
        video.autoplay = true;
        $("#video").empty()
        $(video).appendTo("#video");
        if ($(video).height() > $(video).width()) {
            $(video).css({ 'height': '100%' });
        } else {
            $(video).css({ 'width': '100%' });
        }
        video.onended=function(){reloadBeacons()};

        
        

    }).catch(function(error){
        console.log(error);
    });
}


function reloadBeacons() {
    var exec = require('child_process').exec;
    exec('sudo python3 ./Pages/scanBeacons.py', function(error, stdout, stderr) {
        readBeacons=JSON.parse(stdout);
        var closestBeacon=null;
        for (beacon of readBeacons){
            if(closestBeacon==null){
                closestBeacon=beacon;
            }else{
                if(beacon.rssi< closestBeacon.rssi){
                    
                    closestBeacon=beacon; 
                }
            }
        }
        if(closestBeacon==null&&beaconId==null){
            setTimeout(() => {
                reloadBeacons();
            }, 2000);
          return;
        }
        if(closestBeacon==null&&beaconId!=null){
          location.reload();
        }
        if(beaconId==null||beaconId==undefined){
            beaconId=closestBeacon.id;
            
        }
        if(beaconId != closestBeacon.id){
            console.log("new beacon");
            store.set("beacon",closestBeacon.id)
            location.reload();
        }else{
            console.log("same beacon");
        }
        init();
        if (error !== null) {
            console.log('exec error: ' + error);
        }
    });
}
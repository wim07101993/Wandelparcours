
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
  
    //in second
    

    axios.get(url + "API/v1/residents/bytag/" + beaconId + "/images").then((request) => {
        images = request.data;
        images=shuffle(images);
        createLoop();
        interval = setInterval(createLoop, loopspeed * 1000);
        if (images.length == 0 || images.length == null || images.length == undefined) {
            reloadBeacons();

        }
    }).catch((error) => {
        reloadBeacons();
        console.log(error);
    });
    setTimeout(() => {
        reloadBeacons();
    }, reloadBeaconsSpeed*1000);
}
function createLoop() {
        img = new Image()
        img.onload = function () { fixSize(img) };
        img.src = url + "api/v1/media/" + images[index].id;
        index++;

        if (index > images.length - 1) {
            index = 0;
        }
        console.log("index:" + index);
        console.log("length:" + images.length);
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
          store.delete("beacon");
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




function fixSize(img) {
    $("#images").empty();
    $(img).appendTo("#images");
    if ($(img).height() > $(img).width()) {
        $(img).css({ 'height': '100%' });
    } else {
        $(img).css({ 'width': '100%' });
    }
    console.log($(img).width());
}
/*setTimeout(() => {
    window.location="../index.html";
}, 30000);*/
function shuffle(a) {
    for (let i = a.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [a[i], a[j]] = [a[j], a[i]];
    }
    return a;
}
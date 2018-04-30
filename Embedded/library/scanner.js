//ist.append(dict(id="1", rssi=random.randint(0, 50)))




if (process.send) {
    const Bleacon = require('bleacon');
    var timeout = (process.argv[2]);
    beacons = [];
    Bleacon.startScanning();
    Bleacon.on('discover', function (bleacon) {
        beacons.push(bleacon)
    });
    setTimeout(function(){
        
        process.send(beacons);

    },timeout)
    
}

exports.scanner = function () {
    const Bleacon = require('bleacon');
    var beacons;
    var scanning= false;
    return new Promise((resolve)=>{
        this.scan().then((beacons)=>{
            resolve(beacons);
        });
    });
    this.scan = function () {
        return new Promise((resolve) => {
            try {
                
                function scandata() {
                    beacons = new Map();
                    Bleacon.on('discover', function (bleacon) {
                        beacons.set(bleacon.minor, bleacon)
                    });
                    if(!this.scanning){
                        Bleacon.startScanning();
                        scanning=true;
                    }
                    return new Promise(function (presolve) {
                        setTimeout(function () {
                            var jsonbeacons = [];
                            beacons.forEach(function (val, key, map) {
                                jsonbeacons.push({
                                    id: key,
                                    rssi: val.rssi,
                                    beacon:val
                                })
                            });
                            presolve(jsonbeacons);
                        }, 2000);
    
                    });
                }
                scandata().then(function (beacons) {
                    resolve(beacons);
                });
            } catch (error) {
                resolve("");
            }

        });
    }

}
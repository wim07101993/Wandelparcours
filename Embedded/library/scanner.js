//ist.append(dict(id="1", rssi=random.randint(0, 50)))

exports.scanner = function () {
    return new Promise((resolve)=>{
        scan().then((beacons)=>{
            resolve(beacons);
        });
    });
}
exports.scan = function () {
        const Bleacon = require('bleacon');
        var beacons;
        var scanning= false;
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
                        }, 10000);
    
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


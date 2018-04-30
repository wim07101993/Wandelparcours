import {
    getTrilateration
} from "./trilateration";
import {
    setInterval
} from "timers";
import * as axios from "axios";

export class ChatServer {

    port = 3000;
    scanSpeed = 2500;
    scanned = false;
    beacons = new Map();
    stations = new Map();
    restUrl="http://localhost:5000";
    lastlocation = new Map();
    constructor(clientCode) {
        try {

            this.clientCode = clientCode;
            this.createServer();
            this.stationsLocationObservable();
            this.listen();

        } catch (error) {
            console.log("constructor");
        }
    }

    stationsLocationObservable() {
        let doRequest=()=>{
            console.log("intervalled");
            axios.get(`${this.restUrl}/api/v1/receivermodules`).then((resp)=>{
                
                resp.data.forEach((station)=>{
                    this.stations.set(station.mac,station.position);
                });
            }).catch((e)=>{console.log("axios")});
        }
        doRequest();
        setInterval(async () => {
          doRequest();
        }, 10 * 60 * 1000);
    }
    createServer() {
        try {
            this.server = require('http').createServer();
            this.io = require('socket.io')(this.server, {
                serveClient: false,
                wsEngine: 'ws' // uws is not supported since it is a native module
            });
        } catch (error) {
            console.log(error);
        }
    }

    getPositionForMac(mac) {
        try {
            var position= this.stations.get(mac);
            return {
                x: position.x,
                y: position.y
            };
        } catch (error) {
            console.log("getPositionForMac")
            return "";
        }
    }

    addBeaconsToList(beacons, mac) {

        beacons.forEach(element => {
            try {

                if (this.beacons.has(element.id)) {
                    let beacon = this.beacons.get(element.id);
                    
                    beacon.set(mac, element.beacon.accuracy);
                    this.beacons.set(element.id, beacon);
                } else {
                    var beacon = new Map();
                    beacon.set(mac, element.beacon.accuracy);
                    this.beacons.set(element.id, beacon)
                }
            } catch (error) {
                console.log("addBeaconsToList");
            }

        });
    }

    sortAndConvertMapToJson(map) {

        var scans = [];
        map.forEach((value, key, map) => {
            try {
                var position =this.getPositionForMac(key);
                if(position!=""){
                    scans.push({
                        position: this.getPositionForMac(key),
                        rssi: value
                    });
                }
            } catch (error) {
                console.log("sortAndConvertMapToJson mapforeach");
            }
        });

        try {
            return scans.sort((a, b) => {
                return a.rssi > b.rssi;
            });
        } catch (error) {
            console.log("sortAndConvertMapToJson");
            return "";
        }

    }
    convertBeaconsMapToJson() {
        var jsonBeacons = {};

        this.beacons.forEach((value, key, map) => {
            try {
                var convertedJson = this.sortAndConvertMapToJson(value);
                if (convertedJson != "") {
                    jsonBeacons[key] = convertedJson;
                }
            } catch (error) {
                console.log("convertBeaconsMapToJson");
            }
        });
        console.log(jsonBeacons);
        return jsonBeacons;
    }

    getTrillaterationObject(beacon) {
        try {
            var pos = beacon.position;
            pos.distance = beacon.rssi;
            return pos;
        } catch (error) {
            console.log("getTrillaterationObject");
            return "";
        }
    }

    calculateAndSavePosition() {
        try {
            if (!this.scanned) {
                console.log("calc and save");
                this.scanned = true;
                var jsonBeacons = this.convertBeaconsMapToJson();
                
                for (var beaconIndex in jsonBeacons) {
                    var beacon = (jsonBeacons[beaconIndex]);
                    if (beacon.length >= 3) {
                        console.log((beacon));
                        var pos1 = this.getTrillaterationObject(beacon[0]);
                        var pos2 = this.getTrillaterationObject(beacon[1]);
                        var pos3 = this.getTrillaterationObject(beacon[2]);
                        if (pos1 != "" && pos2 != "" && pos3 != "") {
                            
                            var location = getTrilateration(pos1, pos2, pos3);
                            //12731
                            var lastloaction = this.lastlocation.get(beaconIndex);
                            if(lastloaction!=undefined){
                                if(((new Date).getTime()-lastloaction.date.getTime())<20000){
                                    let x = (lastloaction.location.x+(location.x))/2;
                                    let y = (lastloaction.location.x+(location.y))/2;
                                    this.lastlocation.set(beaconIndex,{location:location,date:new Date()});
                                    location ={x: x,y:y};
                                }else{
                                    this.lastlocation.set(beaconIndex,{location:location,date:new Date()});
                                }
                            }else{
                                this.lastlocation.set(beaconIndex,{location:location,date:new Date()});
                            }
                            console.log(`saving location x:${(location.x)},y:${(location.y)} of beacon:${beaconIndex}`);
                            this.savePositionToDatabase(beaconIndex, location);
                        }
                    }
                }
            }
            console.log("calc and save done");

        } catch (error) {
            console.log("calculateAndSavePosition");
        }
    }

    savePositionToDatabase(tag, location) {
        console.log("saveposition")
        try{
            console.log(`${this.restUrl}/api/v1/location/${tag}/lastlocation/bytag`);
            axios.post(`${this.restUrl}/api/v1/location/${tag}/lastlocation/bytag`,location).then(()=>{
                console.log(`position saved for tag ${tag}`)
            }).catch(()=>{console.log(`no user found for tag${tag}`)});
            
        }catch(e){

            console.log("error on saving");
            console.log(e);

        }


    }

    listen() {
        try {
            this.server.listen(this.port, () => {
                console.log('Running server on port %s', this.port);
            });

            setInterval(() => {
                try {
                    this.io.emit("scan")
                    this.beacons = new Map();
                    this.scanned = false;
                } catch (errorInterval) {
                    console.log("errorInterval");
                }
            }, this.scanSpeed);

            this.io.on('connect', (socket) => {
                try {

                    console.log('Connected client on port %s.', this.port);
                    socket.emit("clientcode", this.clientCode);


                    socket.on("scanned", (data) => {
                        try {
                            this.addBeaconsToList(data.beacons, data.mac);
                            setTimeout(() => {
                                
                                this.calculateAndSavePosition();
                            }, 200);
                        } catch (errorScanned) {
                            console.log("errorScanned")
                        }
                    });
                    socket.on('disconnect', () => {
                        console.log('Client disconnected');
                    });
                } catch (errorSocket) {
                    console.log("errorSocket");
                }
            });
        } catch (error) {
            console.log("listen");
        }


    }

}
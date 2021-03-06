import {
    getTrilateration
} from "./trilateration";
import {
    setInterval
} from "timers";
import * as axios from "axios";

export class ChatServer {

    port = 3000;
    scanSpeed = 5000;
    scanned = false;
    beacons = new Map();
    stations = new Map();
    restUrl="http://localhost:5000";
    username='Modul3';
    password="KioskTo3rmali3n"
    lastlocation = new Map();
    constructor(clientCode) {
        try {

            this.clientCode = clientCode;
            this.createServer();
            this.login();
            this.stationsLocationObservable();
            this.listen();
            

        } catch (error) {
            console.log("constructor");
        }
    }
    /**
     * this function handle's the login
     */
    login(){
        
        const http = axios.create({
          headers: {'userName': this.username,"password":this.password}
        });
        try{
           http.post(`${this.restUrl}/api/v1/tokens`).then((token)=>{
              this.token = token.data.token;
              this.refreshTokenInterval =setInterval(()=>{this.refreshToken()},10*60*1000);
          }).catch((e)=>{
              console.log(e);
          });
        }catch(ex){
            setTimeout(()=>{this.login()},2000);
        }
    
      }
      /**
       * this function logs in every 10 minute, to make sure the token isn't invalid
       */
      refreshToken(){
        const http = axios.create({
          headers: {'userName': this.username,"password":this.password}
        });
        http.post(`${this.restUrl}/api/v1/tokens`).then((result)=>{
          this.token=result.data.token;
          this.level=result.data.user.userType;
        }).catch((e)=>{
          setTimeout(()=>{
            this.refreshToken();
          },10*60*1000);
        });
          
      }
      /**
       * returns a axios instance with the login token
       */
      axios(){
        const instance = axios.create({
          headers: {'token': this.token,'Content-type' : 'application/json'}
        });
        return instance;
      }
      /**
       * this function loads the positions of the stations into a maparray
       */
    stationsLocationObservable() {
        let doRequest=()=>{
            console.log("intervalled");
            this.axios().get(`${this.restUrl}/api/v1/receivermodules`).then((resp)=>{
                
                resp.data.forEach((station)=>{
                    this.stations.set(station.name,station.position);
                });
            }).catch((e)=>{
                setTimeout(() => {
                    doRequest();
                }, 2000);
            });
        }
        doRequest();
        setInterval(async () => {
          doRequest();
        }, 10 * 60 * 1000);
    }
    /**
     * this function creates a listenner for the server
     */
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
    /**
     * this functions returns the position of a station by it's mac/name
     * @param {string} mac this is the mac or name of the station
     */
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
    /**
     * this function adds the beacon read by the station by a key
     * @param  beacons the read value by the station
     * @param {*} name the key for beacon
     */
    addBeaconsToList(beacons, name) {

        beacons.forEach(element => {
            try {
                
                console.log('beacon');
                console.log(element);
                

                if (this.beacons.has(element.id)) {
                    let beacon = this.beacons.get(element.id);
                    
                    beacon.set(name, element.beacon.accuracy);
                    this.beacons.set(element.id, beacon);
                } else {
                    var beacon = new Map();
                    beacon.set(name, element.beacon.accuracy);
                    this.beacons.set(element.id, beacon)
                }
            } catch (error) {
                console.log("addBeaconsToList");
            }

        });
    }

    /**
     * Converts a maparray to a json object, and sorts the stations by distance
     * @param map a mapparray of beacons and stations for every beacon
     */
    sortAndConvertMapToJson(map) {

        var scans = [];
        map.forEach((value, key, map) => {
            try {
                var position =this.getPositionForMac(key);
                if(position!=""){
                    scans.push({
                        position: this.getPositionForMac(key),
                        rssi: value,
                        name:key
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
    /**
     * converts maparray with beacons and keys to a json object
     */
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
    /**
     * @ignore
     */
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
    /**
     * validate position of resident and call save position function
     */
    saveResidentPosition() {
        try {
            if (!this.scanned) {
                console.log("calc and save");
                this.scanned = true;
                var jsonBeacons = this.convertBeaconsMapToJson();
                
                for (var beaconIndex in jsonBeacons) {
                    var beacon = (jsonBeacons[beaconIndex]);
                    var closestStation = beacon[0];
                    console.log("saveClosestStation");
                    this.savePositionToDatabase(beaconIndex, closestStation);
       
                }
            }
            console.log("calc and save done");

        } catch (error) {
            console.log("calculateAndSavePosition");
        }
    }
    /**
     * save the position of the resident to the database
     * @param tag the scanned beacon id
     * @param location  name of the location
     */
    savePositionToDatabase(tag, location) {
        console.log("saveposition")
        try{
            let loc=location.position;
            loc.name=location.name;
            this.axios().put(`${this.restUrl}/api/v1/residents/bytag/${tag}/lastRecordedPosition`,loc).then(()=>{
                console.log(`position saved for tag ${tag}`)
            }).catch(()=>{console.log(`no user found for tag${tag}`)});
            
        }catch(e){

            console.log("error on saving");
            console.log(e);

        }


    }

    /**
     * add listeners to the socket server
     */
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
                            this.addBeaconsToList(data.beacons, data.name);
                            setTimeout(() => {
                                
                                this.saveResidentPosition();
                            }, 1000);
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
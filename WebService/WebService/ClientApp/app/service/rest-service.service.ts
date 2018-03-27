import {Injectable, resolveForwardRef} from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { getBaseUrl } from '../app.module.browser';
import { Resident } from '../models/resident';
import {Station} from "../models/station";
import { CustomErrorHandler } from './customErrorHandler';

@Injectable()
export class RestServiceService {
    [x: string]: any;

    restUrl = "http://localhost:5000/";

    constructor(private http: Http, private customErrorHandler: CustomErrorHandler) {}

    /**
    * Get all residents from database
    * @returns {Resident} residents of type Resident or undefined
    */
    getAllResidents() {
            return new Promise<Resident[]>(resolve => {
                this.http.get(this.restUrl + 'api/v1/residents').subscribe(response => {
                    resolve(<Resident[]>response.json());
                },
                    error => {
                        
                        this.customErrorHandler.updateMessage(error);
                        resolve(undefined);
                        
                    }
                );
            });
        
    }

    /**
    * get one resident and only the needed properties
    * @param uniqueIdentifier unique id of resident
    * @returns {Resident} one resident of type Resident with only the requested properties or undefined   
    */
    getResidentBasedOnId(uniqueIdentifier: string) {
        return new Promise<Resident>(resolve => {
            this.http.get(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + '?properties=firstName&properties=lastName&properties=room&properties=birthday&properties=doctor').subscribe(response => {
                resolve(<Resident>response.json());
            },
                error => {
                    this.customErrorHandler.updateMessage(error);
                    resolve(undefined);
                }
            );
        });
    }



    /**
    * delete resident from database based on id
    * @param uniqueIdentifier unique id of resident
    * @returns message "Deleted" on succes or "Something went wrong" on error
    */

    deleteResidentByUniqueId(uniqueIdentifier: string) {
        return new Promise(resolve => {
            this.http.delete(this.restUrl + 'api/v1/residents/' + uniqueIdentifier).subscribe(response => {
                console.log("Deleted");
                resolve();
            }, error => {
                console.log("Something went wrong");
                this.customErrorHandler.updateMessage(error);
                resolve();
            });
        });
    }

    /**
    * Edit resident in database based on Resident object with properties and only the properties that have been changed
    * @param dataToUpdate Object Resident with all properties
    * @param changedProperties properties != dataToUpdate
    * @returns message "updated" on succes or "Could not update data!" on error.
    */
    editResidentWithData(dataToUpdate: any, changedProperties: any) {
        console.log(dataToUpdate);
        let s: string = "";
        let url: string = "?properties=" + changedProperties[0];
        for (var _i = 1; _i < changedProperties.length; _i++ ){
            url += "&properties=" + changedProperties[_i];
        }

        return new Promise(resolve => {
            this.http.put(this.restUrl + 'api/v1/residents'+url, dataToUpdate).subscribe(response => {
                console.log("updated");
                resolve();
            }, error => {
                console.log("Could not update data!");
                this.customErrorHandler.updateMessage(error);
                resolve();
            });
        });
    }

    /**
    * Add resident to database
    * @param data Object resident with all saved properties
    * @returns True or false based on succes and Console log Message "Saved resident to database" on succes or "Could not save resident to database" on error.
    */
    addResident(data: any){
        console.log(data);
        return new Promise(resolve => {
            this.http.post(this.restUrl + 'api/v1/residents', data).subscribe(response => {
                console.log("Saved resident to database");
                resolve(true);
            }, error => {
                console.log("Could not save resident to database!");
                this.customErrorHandler.updateMessage(error);
                resolve(false);
            });
        });
    }

    /////////
    //MEDIA//
    /////////

    /**
     * Add correct media to database
     * @param uniqueIdentifier string: unique resident ID
     * @param media formdata: media data
     * @param addMedia string: medialink
     * Returns true or false or updates error message
     */
    addCorrectMediaToDatabase(uniqueIdentifier: any, media: any, addMedia: string) {
        return new Promise(resolve => {
            this.http.post(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + addMedia , media).subscribe(response => {
                resolve(true);
            }, error => {
                this.customErrorHandler.updateMessage(error);
                resolve(false);
            });
        });
    }

    /**
     * Get correct media of resident
     * @param uniqueIdentifier string: resident id
     * @param type string: urlLink of media
     * returns true or false--> updates errormessage
     */
    getCorrectMediaOfResidentBasedOnId(uniqueIdentifier: string, type: string) {
        return new Promise<Resident[]>(resolve => {
            this.http.get(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + type).subscribe(response => {
                resolve(<Resident[]>response.json());
            },
                error => {
                    this.customErrorHandler.updateMessage(error);
                    resolve(undefined);
                }
            );
        });
    }
    
    /**
     * delete resident based on uniqueid
     * @param uniqueIdentifier string: Resident id
     * @param uniqueMediaIdentifier string: Media id
     * @param type string: urlLink
     * returns true or false --> updates errormessage
     */
    deleteResidentMediaByUniqueId(uniqueIdentifier: string, uniqueMediaIdentifier: string, type: string) {
        return new Promise(resolve => {
            this.http.delete(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + type + '/' + uniqueMediaIdentifier).subscribe(response => {
                resolve(true);
            }, error => {
                this.customErrorHandler.updateMessage(error);
                resolve(false);
            });
        });
    }

    ////////////////
    //LOCALISATION//
    ////////////////

    async SaveStationToDatabase(station:Station){
        return new Promise(resolve => {

            this.http.post(this.restUrl+"api/v1/receivermodules",station).subscribe(response => {
                    try{
                        resolve(true);
                    }catch (e){
                        resolve(false);
                    }

                },
                error =>{
                    console.log(error);
                    resolve(false);
                }
            )
        });
    }

    async DeleteStation(mac:string){
        return new Promise(resolve => {

            this.http.delete(this.restUrl+"api/v1/receivermodules/"+mac).subscribe(response => {
                    try{
                        resolve(true);
                    }catch (e){
                        resolve(false);
                    }

                },
                error =>{
                    console.log(error);
                    resolve(false);
                }
            )
        });

    }

    UpdateStation(id:string,newMac:string){
        return new Promise(resolve => {
            this.http.put(this.restUrl+"api/v1/receivermodules/"+id+"/Mac",newMac).subscribe(response => {
                resolve(true);
            },error=>{
                resolve(false);
            });
            
        });
    }

    LoadStations(parent:any){
            if (parent.stations!=undefined)
                parent.stations.clear();
            if (parent.renderBuffer.buffer!=undefined)
                parent.renderBuffer.buffer.clear();
            if (parent.stationMacAdresses!=undefined)
                parent.stationMacAdresses=[];
            return new Promise(resolve => {

                this.http.get(this.restUrl+"api/v1/receivermodules").subscribe(response => {

                        let tryParse=<Array<any>>(response.json());

                        let station:any;
                        if (tryParse!=undefined){
                            for (station of tryParse){
                                if (station==undefined)continue;
                                parent.stationMacAdresses.push(station.mac);
                                parent.stations.set(station.mac,station.position);
                                parent.stationsIds.set(station.mac,station.id);
                            }
                        }
                        resolve(true);
                    },
                    error =>{
                        console.log("can't load stations");
                        console.log(error);
                        resolve(false);
                    }
                )
            });
    }
}

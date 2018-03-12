import { Injectable } from '@angular/core';
import { Http, HttpModule, Response} from '@angular/http';
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
        return new Promise<Resident[]>(resolve => {
            this.http.get(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + '?properties=firstName&properties=lastName&properties=room&properties=birthday&properties=doctor').subscribe(response => {
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
    * @returns Message "Saved resident to database" on succes or "Could not save resident to database" on error.
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

    addImagesToDatabase(uniqueIdentifier: any,images: any, options: any = null) {
        return new Promise(resolve => {
            this.http.post(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + '/images/data', images).subscribe(response => {
                console.log(images);
                resolve(true);
            }, error => {
                console.log("Could not update data!");
                this.customErrorHandler.updateMessage(error);
                resolve(false);
            });
        });
    }


    getImagesOfResidentBasedOnId(uniqueIdentifier: string) {
        return new Promise<Resident[]>(resolve => {
            this.http.get(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + '/images').subscribe(response => {
                resolve(<Resident[]>response.json());
            },
                error => {
                    this.customErrorHandler.updateMessage(error);
                    resolve(undefined);
                }
            );
        });
    }


    async SaveStationToDatabase(station:Station){
        return new Promise(resolve => {

            this.http.post(this.restUrl+"api/v1/receivermodules",station).subscribe(response => {
                    try{
                        resolve("success");
                    }catch (e){
                        resolve("error");
                    }

                },
                error =>{
                    console.log(error);
                    resolve("error");
                }
            )
        });
    }

    async DeleteStation(mac:string){
        return new Promise(resolve => {

            this.http.delete(this.restUrl+"api/v1/receivermodules/"+mac).subscribe(response => {
                    try{
                        resolve("success");
                    }catch (e){
                        resolve("error");
                    }

                },
                error =>{
                    console.log(error);
                    resolve("error");
                }
            )
        });

    }


    async LoadStations(parent:any){
        try{
            parent.stations.clear();
            parent.renderBuffer.buffer.clear();
            parent.stationMacAdresses=[];
            return new Promise(resolve => {

                this.http.get(this.restUrl+"api/v1/receivermodules").subscribe(response => {

                        let tryParse=<Array<Station>>(response.json());

                        let station:Station;
                        if (tryParse!=undefined){
                            for (station of tryParse){
                                if (station==undefined)continue;
                                parent.stationMacAdresses.push(station.mac);
                                parent.stations.set(station.mac,station.position);
                            }
                        }
                        resolve(true);
                    },
                    error =>{
                        console.log(error);
                        resolve(true);
                    }
                )
            });
        }catch (e){
            console.log("can't load image");
        }
    }
}
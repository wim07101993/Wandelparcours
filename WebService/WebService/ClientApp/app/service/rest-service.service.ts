import { Injectable } from '@angular/core';
import { Http, HttpModule, Response} from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { getBaseUrl } from '../app.module.browser';
import { Resident } from '../models/resident';

@Injectable()
export class RestServiceService {
    [x: string]: any;

    restUrl = "http://localhost:5000/";

    constructor(private http: Http) {
        
        
    }

    /**
     * Get all residents from database
     */
    getAllResidents() {
      return new Promise<Resident[]>(resolve => {
          this.http.get(this.restUrl +'api/v1/residents').subscribe(response => {
              resolve(<Resident[]>response.json());
          },
              error => {
                  resolve(undefined);
              }
          );
      });   
    }

    /**
     * get one resident and only the needed properties
     * @param uniqueIdentifier
     */
    getResidentBasedOnId(uniqueIdentifier: string) {
        return new Promise<Resident[]>(resolve => {
            this.http.get(this.restUrl + 'api/v1/residents/' + uniqueIdentifier + '?properties=firstName&properties=lastName&properties=room&properties=birthday&properties=doctor').subscribe(response => {
                resolve(<Resident[]>response.json());
            },
                error => {
                    resolve(undefined);
                }
            );
        });
    }



    /**
     * delete resident from database based on id
     * @param uniqueIdentifier
     */

    deleteResidentByUniqueId(uniqueIdentifier: string) {
        return new Promise(resolve => {
            this.http.delete(this.restUrl + 'api/v1/residents/' + uniqueIdentifier).subscribe(response => {
                console.log("Deleted");
                resolve();
            }, error => {
                console.log("Something went wrong");
                resolve();
            });
        });
    }
    /**
     * Update resident in database
     * @param dataToUpdate
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
                resolve();
            });
        });
    }
    
    addResident(data: any){
        console.log(data);
        return new Promise(resolve => {
            this.http.post(this.restUrl + 'api/v1/residents', data).subscribe(response => {
                console.log("updated");
                resolve(true);
            }, error => {
                console.log("Could not update data!");
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
                    resolve(undefined);
                }
            );
        });
    }
}
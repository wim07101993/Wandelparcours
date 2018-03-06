import { Injectable } from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
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

    editResidentWithData(dataToUpdate: any) {
        console.log(dataToUpdate);
        
        return new Promise(resolve => {
            this.http.put(this.restUrl + 'api/v1/residents', dataToUpdate).subscribe(response => {
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
}
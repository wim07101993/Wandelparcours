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

    constructor(private http: Http) {}


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
}

//this.http.get(getBaseUrl() + 'api/v1/residents')
      //    .map(
      //    (response: Response) => {
      //        this.residents = [];
      //        const data = response.json();
      //        for (const resident of data) {
      //            let residentObj = <Resident>resident;
      //            this.residents.push(residentObj);
      //        }
      //        return this.residents;
      //    })
          //.map((res: Response) => res.json());
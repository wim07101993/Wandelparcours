import { Injectable } from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
import 'rxjs/add/operator/map'

@Injectable()
export class RestServiceService {

  constructor(private http: Http) { }

  testData() {
      return "aaaa";
  }
  getAllResidents() {
      return this.http.get('http://10.9.4.35:3000/api/v1/residents').map((res: Response) => res.json());
  }
}

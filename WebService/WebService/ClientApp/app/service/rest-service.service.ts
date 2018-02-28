import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/Http'

@Injectable()
export class RestServiceService {

  constructor(private http: Http) { }

}

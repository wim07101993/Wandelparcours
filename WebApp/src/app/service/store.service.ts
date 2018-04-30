import { Injectable } from '@angular/core';

@Injectable()
export class StoreService {
  data=new Map();
  constructor() { 

  }
  Set(key:string, value:any){
    this.data.set(key,value);
    console.log(this.data);
  }

  Get(key:string){
    return this.data.get(key);
  }
}

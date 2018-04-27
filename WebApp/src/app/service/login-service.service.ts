import { Injectable } from '@angular/core';
import axios from 'axios';
import { Router } from '@angular/router';

@Injectable()
export class LoginService {

  private token:string;
  private username:string;
  private password:string;
  private level:number;
  
  surfUrl:string="/";
  constructor(private router: Router) { }
  


  async login(username:string,password:string){
    const http = axios.create({
      headers: {'userName': username,"password":password}
    });
    try{
      let token =  await http.post("/api/v1/tokens");
      this.token = token.data;
      setInterval(()=>{this.refreshToken()},15*60*1000);
      return true
    }catch(ex){return false}

  }

  checkLogin(){
    if(this.router.url!="/login"&& this.token ==null){
      this.surfUrl= this._window.location.pathname;
      this.router.navigate(["/login"]);
    }
  }


  checkCookies(){
    
  }

  get _window ():any{
    return window;
  }

  refreshToken(){
    const http = axios.create({
      headers: {'userName': this.username,"password":this.password}
    });
    http.post("/api/v1/tokens").then((token)=>{
      this.token=token.data;
    }).catch(()=>{
      setTimeout(()=>{
        this.refreshToken();
      },60*1000);
    });
      
  }


  get axios(){
    const instance = axios.create({
      headers: {'token': this.token}
    });
    return instance;
  }
}

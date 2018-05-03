import { Injectable } from '@angular/core';
import axios from 'axios';
import { Router } from '@angular/router';

@Injectable()
export class LoginService {

  private token:string;
  private username:string;
  private password:string;
  private level:number;
  public acl:number;
  surfUrl:string="/";
  constructor(private router: Router) { }
  


  async login(username:string,password:string){
    this.username=username;
    this.password=password;
    const http = axios.create({
      headers: {'userName': username,"password":password}
    });
    try{
      let result =  await http.post("/api/v1/tokens");
      this.token=result.data.token;
      this.acl=result.data.user.userType;
      
      setInterval(()=>{this.refreshToken()},15*60*1000);
      //15*60*1000
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
    http.post("/api/v1/tokens").then((result)=>{
      this.token=result.data.token;
      this.acl=result.data.user.userType;
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

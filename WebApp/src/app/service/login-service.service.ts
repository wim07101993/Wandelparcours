import { Injectable } from '@angular/core';
import axios from 'axios';
import { Router } from '@angular/router';

@Injectable()
export class LoginService {

  public token:string;
  public username:string;
  private password:string;
  public level:number;
  private refreshTokenInterval:NodeJS.Timer;
  surfUrl:string="/";
  constructor(private router: Router) { }
  
/**
 * this function logs u out of the system
 */
  logout(){
    this.token=null;
    this.username=null;
    this.password=null;
    this.level=null;
    clearInterval(this.refreshTokenInterval);
    this.deleteAllCookies();
    document.location.href = "/login";
  }

/**
 * @ignore
 */
  deleteAllCookies() {
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
  }
  /**
   * This function is to ask the server a token for the given user
   * @param username 
   * @param password 
   */
  async login(username:string,password:string){
    this.username=username;
    this.password=password;
    const http = axios.create({
      headers: {'userName': username,"password":password}
    });
    try{
      let token =  await http.post("/api/v1/tokens");
      this.token = token.data.token;
      this.level= token.data.user.userType;
      this.refreshTokenInterval =setInterval(()=>{this.refreshToken()},15*60*1000);
      return true
    }catch(ex){return false}

  }
  /**
   * this functin check's if your loged in
   */
  checkLogin(){
    if(this.router.url!="/login"&& this.token ==null){
      this.surfUrl= this._window.location.pathname;
      this.router.navigate(["/login"]);
      
    }
  }

/**
 * @ignore
 */
  checkCookies(){
    
  }
/**
 * @ignore
 */
  get _window ():any{
    return window;
  }

/**
 * this function refreshes the token every 20 min
 */
  refreshToken(){
    const http = axios.create({
      headers: {'userName': this.username,"password":this.password}
    });
    http.post("/api/v1/tokens").then((result)=>{
      this.token=result.data.token;
      this.level=result.data.user.userType;
    }).catch(()=>{
      setTimeout(()=>{
        this.refreshToken();
      },60*1000);
    });
      
  }

  /**
   * this getter returns an axios instance with a login token
   */
  get axios(){
    const instance = axios.create({
      headers: {'token': this.token,'Content-type' : 'application/json'}
    });
    return instance;
  }
}

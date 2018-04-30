import { Component, OnInit } from '@angular/core';
import 'assets/login-animation.js';
declare var $: any;
declare var Materialize: any;
import { LoginService } from "../../service/login-service.service";
import axios from 'axios';
import { Router } from '@angular/router';
import {  StoreService } from "../../service/store.service";
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

export class LoginComponent implements OnInit {

  private email: string;
  private password: string;
  private rememberMe:boolean=false;
  private waitingResponse=false;

  cookieLaw=true;
  constructor(private loginService:LoginService, private router:Router,private store:StoreService) {
    let cookie = this.getCookie("login");
    if(cookie!=null){
      let login =JSON.parse(window.atob(cookie));
      this.email=login.username;
      this.password=login.password;
      this.login();
    }

  }

  ngOnInit() {}


  ngAfterViewInit() {
    (window as any).initialize();
  }

  

  login(){
    this.waitingResponse=true;
    this.loginService.login(this.email,this.password).then((result)=>
    {
      this.waitingResponse=false;
      if(result==true){
        let redirectURL= this.loginService.surfUrl=="/login"? "/":this.loginService.surfUrl; 
        this.router.navigate([redirectURL]);
        this.store.Set("acl",0);
        if(this.rememberMe){
          let login:any = {username:this.email,password:this.password};
          login = JSON.stringify(login);
          login = window.btoa(login);
          this.setCookie("login",login,30);
        }
      }else{
        alert("ga weg lan");
      }
    }
    ).catch(()=>{this.waitingResponse=false});
  }

 setCookie(name,value,days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days*24*60*60*1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "")  + expires + "; path=/";
}
getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
        var c = ca[i];
        while (c.charAt(0)==' ') c = c.substring(1,c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
    }
    return null;
}


  cancelFormEvent(empForm: any, event: Event) {
    event.preventDefault();
    this.login();
    
  }
}

/*
SysAdmin    0,
Nurse       1,
User        2,
Module      3,
Guest       4
  */

import {Component} from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {MediaService} from '../../service/media.service';
import { Router,NavigationStart } from '@angular/router';
import { LoginService } from "../../service/login-service.service";
import { StoreService } from "../../service/store.service";
declare var $: any;
import 'rxjs/add/operator/filter';
import { CheckboxControlValueAccessor } from '@angular/forms';
import { promise } from 'protractor';


@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [RestServiceService, MediaService]
})
export class AppComponent {
  accessPerRoute=[
    {url:"/modules","acl":[0]},
    {url:"/residents","acl":[0,1,2]}  ,
    {url:"/resident/","acl":[0,1,2]} ,
    {url:"/users","acl":[0]},

  
  ]
  constructor(private router:Router,private loginService:LoginService,private store:StoreService){
    this.loginService.checkLogin();
    this.router.events.filter(event => event instanceof NavigationStart)
    .subscribe((event:NavigationStart)=>{
      this.checkAcces(event.url);
    });
  }
  async checkAcces(url:string){
    try {
      
      await this.awaitTime(100);
      let acl = this.store.Get("acl"); 
      
      let route =this.accessPerRoute.find((apr)=>{
        return url.indexOf(apr.url)>=0;
      });
      
      let authorized=route.acl.includes(acl);
      if(!authorized){
        this.router.navigate(["/residents"]);
      }
    } catch (error) {
      this.router.navigate(["/residents"]);
    }
    
  }

    awaitTime(timeInMs){
      return new Promise(resolve=>{
        setTimeout(()=>{
          resolve();
        },timeInMs);
      });

    }
}


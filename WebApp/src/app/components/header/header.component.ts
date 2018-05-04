import {Component, OnInit} from '@angular/core';
import { LoginService } from "../../service/login-service.service";
declare var $: any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  //user = 'Beheerder';
  pageTitle = 'Toermalien';
  initialized=false;
  constructor(private login:LoginService) {
  }

  openSideNav(){
    $('.button-collapse').sideNav('show');

  }

  get user(){
    return this.login.username;
  }
  ngOnInit() {
        // Initialize collapse button
        if(this.initialized==false){
          $(".button-collapse").sideNav();
          this.initialized=true;
        }
        
        
        // Initialize collapsible (uncomment the line below if you use the dropdown variation)
        $('.button-collapse').collapsible();
  }
}

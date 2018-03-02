import { Component, OnInit } from '@angular/core';
declare var $:any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

    gebruiker = 'Beheerder';
    pageTitle = 'Pagina Titel'

    constructor() { 
    }

    ngOnInit() {
  }

  /* function to Open and Close the sideNav */
  openSideNav(){
    $('.button-collapse').sideNav({
                menuWidth: 240,
                closeOnClick: true
            });
            $('.collapsible').collapsible();
  }

}

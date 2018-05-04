import {Component, OnInit} from '@angular/core';

declare var $: any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  user = 'Beheerder';
  pageTitle = 'Pagina Titel';
  initialized=false;
  constructor() {
  }

  openSideNav(){
    $('.button-collapse').sideNav('show');

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

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
  
  constructor() {
  }

  openSideNav(){
    $('.button-collapse').sideNav('show');
  }

  ngOnInit() {
        // Initialize collapse button
        $(".button-collapse").sideNav();
        // Initialize collapsible (uncomment the line below if you use the dropdown variation)
        $('.button-collapse').collapsible();
  }
}

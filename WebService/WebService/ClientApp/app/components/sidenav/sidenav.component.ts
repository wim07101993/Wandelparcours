import { Component, OnInit } from '@angular/core';
declare var $:any;

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.css']
})
export class SidenavComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  
  }

  closeSideNav(){
      $('.button-collapse').sideNav({
              menuWidth: 300, // Default is 300
              closeOnClick: true, // Closes side-nav on <a> clicks, useful for Angular/Meteor
              draggable: true // Choose whether you can drag to open on touch screen
          }
      );
  }
   

  

}

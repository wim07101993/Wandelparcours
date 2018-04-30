import {Component, OnInit} from '@angular/core';
import { StoreService } from "../../service/store.service";
declare var $:any;

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.css']
})
export class SidenavComponent implements OnInit {
  links=[
    {url:["/residents"],acl:[0,1,2],icon:"people",text:"Bewoners"},
    {url:["/users"],acl:[0],icon:"verified_user",text:"Gebruikers"},
    {url:["/modules"],acl:[0],icon:"location_on",text:"Modules"},
    {url:["/tracking"],acl:[0],icon:"accessibility",text:"Tracking"},
  ]
  

  

  constructor(private store:StoreService) { }

  ngOnInit() {

  }
  get listUrlForAcl(){
    //*ngIf="link.acl.includes(acl)!=undefined'"
    let acl = this.store.Get("acl");
    acl = acl==undefined ? 4:acl;
    let links = this.links.filter((link)=>{
      return link.acl.includes(acl);
    });
    return links;
  }
  closeSideNav(){
      $('.button-collapse').sideNav('hide');
    setTimeout(() => {
      $('.button-collapse').sideNav('hide');
    }, (600));
  }
}

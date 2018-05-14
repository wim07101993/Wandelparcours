import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {RestServiceService} from '../../../service/rest-service.service';
/**
 * @ignore
 */
@Component({
  selector: 'app-tracking',
  templateUrl: './tracking.component.html',
  styleUrls: ['./tracking.component.css']
})
export class TrackingComponent implements OnInit {
  loaded:any;
  userText="Bewoner bevindt zich momenteel in een niet-detecteerbaar zone.";
  id:any;
  constructor(private service: RestServiceService,private route:ActivatedRoute) { }

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];
    setInterval(()=>{this.loadResidentLocation()},30*1000)
    this.loadResidentLocation();
  }
  
  
  /**
   * Load location for current resident.
   */
  async loadResidentLocation(){
    this.loaded = await this.service.getOneResidentWithAKnownLastLocation(this.id);
    var today = new Date();
    console.log(this.loaded.lastRecordedPosition);
    let scanned =new Date(Date.parse(this.loaded.lastRecordedPosition.timeStamp));
    
    var diffMs = (today.getTime()-scanned.getTime() ); // milliseconds between now & Christmas
    var diffMins = Math.round(diffMs / 60000); // minutes
    if(diffMins==0){
      diffMins=1;
    }
    if(diffMins>60){
      this.userText="Bewoner bevindt zich momenteel in een niet-detecteerbaar zone.";
    }else{
      this.userText=`Bewoner werd ${diffMins}min geleden gedetecteerd in zone: ${this.loaded.lastRecordedPosition.name}`;
    }
  }

}

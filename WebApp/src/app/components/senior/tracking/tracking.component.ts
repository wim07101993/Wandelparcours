import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {RestServiceService} from '../../../service/rest-service.service';
@Component({
  selector: 'app-tracking',
  templateUrl: './tracking.component.html',
  styleUrls: ['./tracking.component.css']
})
export class TrackingComponent implements OnInit {
  loaded:any;
  userText="Bewoner kan niet getracked worden.";
  id:any;
  constructor(private service: RestServiceService,private route:ActivatedRoute) { }

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];
    setInterval(()=>{this.loadResidentLocation()},30*1000)
    this.loadResidentLocation();
  }
  
  
  async loadResidentLocation(){
    this.loaded = await this.service.getOneResidentWithAKnownLastLocation(this.id);
    this.userText=`Bewoner bevind zich in zone: ${this.loaded.lastRecordedPosition.name}`;
  }

}

import { Component, OnInit } from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  constructor(private service: RestServiceService) { }

  ngOnInit() {
  }

  
}

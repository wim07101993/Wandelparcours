import { Component, OnInit } from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {NgForm} from '@angular/forms';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  constructor(private service: RestServiceService) { }

  ngOnInit() {
    this.getUsers()
  }

  async getUsers(){
    const users  = await this.service.getUsers();
    console.log(users);
  }

  async deleteUser(userId : string){
    await this.service.deleteUser(userId);
    this.getUsers();
  }

  createUser(form : NgForm){
    const data = {};

    this.service.createUser("","","");
  }

}

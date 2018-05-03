import { Component, OnInit } from '@angular/core';
import {NgForm} from '@angular/forms';
declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

    /**
   * Reset the form on close
   * @param form of type NgForm
   */
  resetForm(form: NgForm) {
    form.reset();
  }
  

  openAddUserModal() {
    $('#add-user-modal').modal();
    $('#add-user-modal').modal('open');
  }
}

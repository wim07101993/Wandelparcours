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
  password:string;
  passwordcheck:string;
  constructor() { }

  ngOnInit() {
    $('select').material_select();
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

  validatePassword(){

    console.log(this.password);
    console.log(this.passwordcheck);
    if (this.password !== this.passwordcheck){
      alert('Wachtwoorden komen niet overeen!')
    }
    else{
      alert('Wachtwoorden komen wel overeen!')
    }
  }
}

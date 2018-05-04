import { Component, OnInit } from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {NgForm} from '@angular/forms';
declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

    constructor(private service: RestServiceService) {
    }

    async getUsers() {
        const users = await this.service.getUsers();
        console.log(users);
    }

    async deleteUser(userId: string) {
        await this.service.deleteUser(userId);
        this.getUsers();
    }

    createUser(form: NgForm) {
        const data = {
            userName: form.value.userName,
            email: form.value.email,
            userType: form.value.userType,
            userPassword: form.value.password1
    };

        console.log(data);
        this.service.createUser(data.userName,data.userPassword,data.userType,data.email);
    }

    password: string;
    passwordcheck: string;

    ngOnInit() {
        this.getUsers()
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

    validatePassword() {

        console.log(this.password);
        console.log(this.passwordcheck);
        if (this.password !== this.passwordcheck) {
            alert('Wachtwoorden komen niet overeen!')
        }
        else {
            alert('Wachtwoorden komen wel overeen!')
        }
    }

}


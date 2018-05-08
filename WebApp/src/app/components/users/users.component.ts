import { Component, OnInit } from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {NgForm} from '@angular/forms';
import {user} from '../../models/user';
import {Resident} from '../../models/resident';

declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    users : user;
    term: any = null;
    search = false;
    userModal: user;

    constructor(private service: RestServiceService) {
        this.userModal = <user>{};
        this.getUsers()
    }


    async getUsers() {
        this.users = await this.service.getUsers();
        console.log(this.users);
    }

    async deleteUser(userId: string) {
        await this.service.deleteUser(userId);
        this.getUsers();
    }

    async createUser(form: NgForm) {
        const data = {
            userName: form.value.userName,
            email: form.value.email,
            userType: form.value.userType,
            userPassword: form.value.password1
    };

        await this.service.createUser(data.userName,data.userPassword,data.userType,data.email);
        form.reset();
        // close modal/form and 'reload' page
        setTimeout(() => {
            $('#add-user-modal').modal('close');
        }, 200);
        this.getUsers();
    }

    password: string;
    passwordcheck: string;

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

    /**
     * Focus to input for the searchbar --> input will be active if event 'button' has been pressed
     */
    focusInput() {
        setTimeout(() => {
            $('#focusToInput').focus();
        }, 200);
    }

    /**
     * Opens modal
     * @param modalResident
     */
    openModal(user: user) {
        this.userModal = user;
        $('#deleteModalUser').modal();
        $('#deleteModalUser').modal('open');
    }
    /**
     * Close modal
     */
    closeModal() {
        $().modal('close');
    }

}


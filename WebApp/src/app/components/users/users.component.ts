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
    userTypes = [
        {"id":0,"itemName":"Admininstrator"},
        {"id":1,"itemName":"Zorgkundige"},
        {"id":2,"itemName":"Gebruiker"}
      ];
      createUserModel=new formUser();

      createUserType=[];
      settings =  {singleSelection: true, text:"ToegangsLevel"};
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

    SelectCreateType(event){
        this.createUserModel.userType=event.id;
    }
    createUser() {
        console.log(this.createUserModel);
        if(this.createUserModel.userName==""){
            Materialize.toast('Vul Gebruikersnaam in!', 3000);
            return;
        }
        if(this.createUserModel.email==""){
            Materialize.toast('Vul Email in!', 3000);
            return;
        }
        if(this.createUserModel.userType==99){
            Materialize.toast('Selecteer gebruikers type!', 3000);
            return;
        }
        if(this.createUserModel.userPassword==""){
            Materialize.toast('Vul Wachtwoord in!', 3000);
            return;
        }
        if(this.createUserModel.userPassword!=this.createUserModel.verPassword){
            Materialize.toast('Wachtwoorden komen niet overeen!', 3000);
            return;
        }
        
        this.service.createUser(this.createUserModel.userName,this.createUserModel.userPassword,this.createUserModel.userType,this.createUserModel.email);
        this.createUserModel= new formUser();
        
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


class formUser{
        userName="";
        email="";
        userType=99;
        userPassword= "";
        verPassword= "";
        userTypeModel=[];
    constructor(){
        this.userName="";
        this.email="";
        this.userType=99;
        this.userPassword= "";
        this.verPassword= "";
        this.userTypeModel=[];
    }
}
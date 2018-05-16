import { Component, OnInit, NgZone } from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {NgForm} from '@angular/forms';
import {user} from '../../models/user';
import {Resident} from '../../models/resident';
import { forEach } from '@angular/router/src/utils/collection';

declare var $: any;
declare var Materialize: any;

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    residents: any =  Resident;
    users : user;
    term: any = null;
    search = false;
    userModal: user;
    userTypes = [
        {"id":0,"itemName":"Admininstrator"},
        {"id":1,"itemName":"Zorgkundige"},
        {"id":2,"itemName":"Gebruiker"}
    ];
    residentsList = [];
    settingsResident =  {singleSelection: false, text:"Bewoner(s) selecteren"};
    selectedItems=[];
    
    createUserModel=new formUser();
    editUserModel=new formUser();
    createUserType=[];
    settings =  {singleSelection: true, text:"ToegangsLevel"};
    updateUser: user;
    
    residentToUserId="";
    residentToUserList=new Map<string,string>();
    loadingOverlay : boolean=false;
    password: string;
    passwordcheck: string;
    constructor(private service: RestServiceService,private app: NgZone) {
        this.userModal = <user>{};
        
    }

    async ngOnInit() {
        this.loadingOverlay=true;
        try{
            await this.getUsers();
            $('select').material_select();
            await this.showAllResidents();
            this.loadingOverlay=false;
        }catch(ex){
            console.log(ex);
            this.loadingOverlay=false;
        }

        this.loadingOverlay=false;
    }

    /**
     * Get all users
     * @returns {Promise<void>}
     */
    async getUsers() {
        this.users = await this.service.getUsers();
    }

    /**
     * Delete a user based on id
     * @param {string} userId
     * @returns {Promise<void>}
     */
    async deleteUser(userId: string) {
        await this.service.deleteUser(userId);
        Materialize.toast('User succesvol verwijderd!',5000);
        this.getUsers();
    }

    /**
     * Select a type (admin,...°
     * @param event
     * @constructor
     */
    SelectCreateType(event){
        this.createUserModel.userType=event.id;
    }

    /**
     * Select to edit type
     * @param event
     * @constructor
     */
    SelectEditType(event){
        this.editUserModel.userType=event.id;
    }

    /**
     * Opens user edit modal with user
     * @param {user} users
     */
    editUserModal(users:user){
        this.editUserModel.id=users.id;
        this.editUserModel.email=users.email;
        this.editUserModel.userType=users.userType;
        this.editUserModel.userName=users.userName;
        $('#edit-user-modal').modal();
        $('#edit-user-modal').modal('open');
    }

    /**
     * Create a new user with certain privileges
     * @returns {Promise<void>}
     */
    async createUser() {
        console.log(this.createUserModel);
        if(this.createUserModel.userName==""){
            Materialize.toast('Vul Gebruikersnaam in!', 3000);
            return;
        }
        if(this.createUserModel.email==""){
            Materialize.toast('Vul Email in!', 3000);
            return;
        }
        if(this.createUserModel.userType=="99"){
            Materialize.toast('Selecteer gebruikers type!', 3000);
            return;
        }
        if(this.createUserModel.password==""){
            Materialize.toast('Vul Wachtwoord in!', 3000);
            return;
        }
        if(this.createUserModel.password!=this.createUserModel.verPassword){
            Materialize.toast('Wachtwoorden komen niet overeen!', 3000);
            return;
        }
        
        await this.service.createUser(this.createUserModel.userName,this.createUserModel.password,this.createUserModel.userType,this.createUserModel.email);
        this.createUserModel= new formUser();
        Materialize.toast('User succesvol toegevoegd!',5000);
        // close modal/form and 'reload' page
        setTimeout(() => {
            $('#add-user-modal').modal('close');
        }, 200);

        this.ngOnInit();
    }

     /**
     * get all residents async from service
     */
    async showAllResidents() {
        const residents: any = await this.service.getAllResidents();
        this.residentsList = [];
        console.log(residents);
        console.log(residents[0].firstName)

        for (let resident of residents){
            // TODO Sorteren per category=wooneenheid 
            let listObject = {id:resident.id,itemName:resident.firstName+' '+resident.lastName, sortObject:""};
            this.residentsList.push(listObject);
        }

    }

    /**
     * Reset the form
     */
    resetForm() {
        console.log("Test");
    }

    /**
     * Opens the add user modal
     */
    openAddUserModal() {
        $('#add-user-modal').modal();
        $('#add-user-modal').modal('open');
    }

    /**
     * Validate password  with the form
     */
    validatePassword() {
        if (this.password !== this.passwordcheck) {
            alert('Wachtwoorden komen niet overeen!')
        }
        else {
            alert('Wachtwoorden komen wel overeen!')
        }
    }

    /**
     * focusInput to searchbar
     */
    focusInput() {
        setTimeout(() => {
            $('#focusToInput').focus();
        }, 200);
    }

    /**
     * Opens modal with user
     * @param {user} user
     */
    openModal(user: user) {
        this.userModal = user;
        $('#deleteModalUser').modal();
        $('#deleteModalUser').modal('open');
    }

    /**
     * Closes modal
     */
    closeModal() {
        $().modal('close');
    }

    /**
     * Edit the user
     * @param {user} userData
     * @returns {Promise<void>}
     */
    async editUser(userData: user){
        this.updateUser=userData;
        //this.updateUser.id = userData.id;
        const changedProperties = [];
        for (const prop in this.updateUser) {
            if (this.updateUser[prop] != null && this.updateUser[prop] !== '' && (this.updateUser[prop] != 'usertypeModel')) {
                changedProperties.push(prop);
            }
        }
        changedProperties.splice((changedProperties.length -1));
        const updateData = this.updateUser;
        delete updateData['usertypeModal'];


        await this.service.updateUser(updateData, changedProperties);
        this.updateUser = <user>{};


         setTimeout(() => {
            $('#edit-user-modal').modal('close');
        }, 200);
        Materialize.toast('User succesvol geüpdate!',5000);
        // get all users again after updating
        this.getUsers();
    }

    /**
     * Opens the add residents to user modal
     * @param user
     */
    openResToUserModal(user){
        this.residentToUserId=user.id;
        
        this.updateUserList(user);
        $('#residentToUserModal').modal();
        $('#residentToUserModal').modal('open');
    }

    /**
     * Updates user user list
     * @param user
     */
    updateUserList(user){
        this.residentToUserList.clear();
        this.selectedItems=[];
        user.residents.forEach((residentId)=>{
            let resident = this.residentsList.find((x)=>{return x.id==residentId});
            this.selectedItems.push(resident);
            this.residentToUserList.set(residentId,resident);
        });

        
        return;
    }

    /**
     * Observer checks changes in selected resident
     * @param event
     * @constructor
     */
    OnResidentSelect(event){
        this.residentToUserList.set(event.id,event);
    }

    /**
     * Observer checks changes in selected resident
     * @param event
     * @constructor
     */
    OnResidentDeSelect(event){
        this.residentToUserList.delete(event.id);
    }

    /**
     * Link a user to a resident <--> link resident to a user
     * @returns {Promise<void>}
     */
    async linkUsersToResident(){
        this.loadingOverlay=true;
        try{
        let usersIds=  Array.from(this.residentToUserList.keys());
        let cleared=false;
        while(cleared==false){
            cleared=await this.service.clearResidentsFromUser(this.residentToUserId);
        }
        await usersIds.forEach( async (resId)=>{
            let result=false;
            while(result==false){
                result = await this.service.linkResidentToUser(this.residentToUserId,resId);
            }
            
            
        });
        }catch(ex){
            this.loadingOverlay=false;
        }
        this.ngOnInit();
    }

    /**
     * Selects all residents
     * @param event
     */
    onSelectAllResidents(event){
        console.log(event);
        event.forEach((resident)=>{
            this.residentToUserList.set(resident.id,resident);
        })
    }

    /**
     * Deselect all residents
     */
    onDeSelectAllResidents(){
        this.residentToUserList.clear();
    }

}


class formUser{
        id="";
        userName="";
        email="";
        userType="99";
        password= "";
        verPassword= "";
    constructor(){
        this.id="";
        this.userName="";
        this.email="";
        this.userType="99";
        this.password= "";
        this.verPassword= "";
    }
}
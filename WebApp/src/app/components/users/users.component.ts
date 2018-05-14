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
    async getUsers() {
        this.users = await this.service.getUsers();
    }

    async deleteUser(userId: string) {
        await this.service.deleteUser(userId);
        Materialize.toast('User succesvol verwijderd!',5000);
        this.getUsers();
    }

    SelectCreateType(event){
        this.createUserModel.userType=event.id;
    }
    SelectEditType(event){
        this.editUserModel.userType=event.id;
    }

    editUserModal(users:user){
        this.editUserModel.id=users.id;
        this.editUserModel.email=users.email;
        this.editUserModel.userType=users.userType;
        this.editUserModel.userName=users.userName;
        $('#edit-user-modal').modal();
        $('#edit-user-modal').modal('open');
    }

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
     * Reset the form on close
     * @param form of type NgForm
     */
    resetForm() {
        console.log("Test");
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
     * @param modalUser
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
     * Opens residentToUsermodal
     */
    openResToUserModal(user){
        this.residentToUserId=user.id;
        
        this.updateUserList(user);
        $('#residentToUserModal').modal();
        $('#residentToUserModal').modal('open');
    }
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
    OnResidentSelect(event){
        this.residentToUserList.set(event.id,event);
    }
    OnResidentDeSelect(event){
        this.residentToUserList.delete(event.id);
    }
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
    onSelectAllResidents(event){
        console.log(event);
        event.forEach((resident)=>{
            this.residentToUserList.set(resident.id,resident);
        })
    }
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
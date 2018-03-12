import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'
import { Ng2SearchPipeModule } from 'ng2-search-filter'
import { async } from '@angular/core/testing';
import {NgForm} from "@angular/forms";
import { Router } from '@angular/router';
declare var $:any;
declare var Materialize:any;

@Component({
    selector: 'app-residents',
    templateUrl: './residents.component.html',
    styleUrls: ['./residents.component.css'],
    providers: [RestServiceService]
})

export class ResidentsComponent implements OnInit {
    ngOnInit(): void {
    }

    view: string = "card-view";
    data: any = null;
    residents: Resident[];
    modalResident: Resident;
    updateResident: any;
    search: boolean = false;

    /**
     * Injects the service and router
     * @param service Restservice
     * @param router Router
     */
    constructor(private service: RestServiceService, private router: Router) {
        this.showAllResidents();
        this.residents = [];

        /*Creates empty modals to avoid collision with previous existing modals should they not be deleted*/
        this.modalResident = <Resident>{
            firstName: "", lastName: "", room: "", id: "", birthday: new Date(), doctor: { name: "", phoneNumber: "" }
        };
        this.updateResident = {
            firstName: "", lastName: "", room: "", id: "", birthday: "", doctor: { name: "", phoneNumber: "" }
        };

    }

    /**
    * Focus to input for the searchbar --> input will be active if event 'button' has been pressed
    */
    focusInput() {
        let a: any;
        setTimeout(() => {
            $('#focusToInput').focus();
        }, 200);

    }

    /**
     * Opens modal
     * @param modalResident
     */
    openModal(modalResident: Resident) {
        //alert(uniqueIdentifier);
        this.modalResident = modalResident;
        $("#deleteModalResident").modal();
        $("#deleteModalResident").modal("open");
    }
    /**
     * Open modal in edit mode and fill modal with resident
     * @param modalResident
     */
    openEditModal(modalResident: Resident) {
        this.modalResident = modalResident;
        $("#editModalResident").modal();
        $("#editModalResident").modal("open");
        $('.datepicker').pickadate(
            {
                clearDate: true,
                monthsFull: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'December'],
                monthsShort: ['Jan', 'Feb', 'Maa', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
                weekdaysShort: ['ma', 'di', 'wo', 'do', 'vr', 'zat', 'zon'],
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 120, // Creates a dropdown of 15 years to control year,
                max: new Date(),
                today: 'Vandaag',
                clear: 'Wissen',
                close: 'Bevestigen',
                closeOnSelect: true,
                format: 'mm-dd-yyyy'
            });
    }
    /**
     * Close modal
     */

    closeModal() {
        $().modal("close");
    }

    /**
     * get all residents async from service
     */
    async showAllResidents() {
        let residents: any = await this.service.getAllResidents();
        console.log(residents);
        if (residents != undefined)
            this.residents = residents;
        else {
            alert("oops! :( looks like something went wrong :(");
        }
    }

    /**
     * Delete resident async based on unique identifier --> "ID"
     * @param uniqueIdentifier
     */

    async deleteResident(uniqueIdentifier: string) {
        await this.service.deleteResidentByUniqueId(uniqueIdentifier);
        this.showAllResidents();
    }

    /**
     * Edit and save resident from service
     * @param resident of type Resident
     */

    async editResident(resident: Resident) {
        //get correct ID
        this.updateResident.id = resident.id;
        let birthDay = $("#birthDay").val();

        console.log(birthDay);

        //if birthday hasn't been entered make sure birthday is of type Date
        if (birthDay != "") {
            //console.log("update birthday");
            let a = new Date(birthDay);
            console.log(a);
            this.updateResident.birthday = a;
        }
        /**
         * Send resident object and the changed properties
         */
        let changedProperties = [];
        for (let prop in this.updateResident) {
            if (this.updateResident[prop] != null && this.updateResident[prop] != "") {
                changedProperties.push(prop);
            }
        }
        if (this.updateResident.doctor.name == "") {
            this.updateResident.doctor.name = this.modalResident.doctor.name;
        }
        if (this.updateResident.doctor.phoneNumber == "") {
            this.updateResident.doctor.phoneNumber = this.modalResident.doctor.phoneNumber;
        }
        if (this.updateResident.birthday == "") {
            this.updateResident.birthday = this.modalResident.birthday;
        }
        console.log(changedProperties);

        $('#birthDay').val("");

        let updateData = this.updateResident;

        await this.service.editResidentWithData(updateData, changedProperties);

        this.updateResident = {
            firstName: "", lastName: "", room: "", id: "", birthday: "", doctor: { name: "", phoneNumber: "" }
        };

        //get all residents again after updating
        this.showAllResidents();
    }

    cleanForm(){}
        
   /**
    * Add resident to database
    * @param form of type NgForm
    */   
   async addResident(form: NgForm){

        // Workaround for dateformat
        let birthday = $("#abirthdate").val();
        let a;
        if (birthday != "") {
            a = new Date(birthday);
        }

        // Get data from form-inputs
         let data = {
             firstName: form.value.aFirstName, 
             lastName: form.value.aLastName, 
             room: form.value.aRoom, 
             birthday: a, 
             doctor: { name: form.value.aDoctor, phoneNumber: form.value.aTelefoon }
         };

        // Send gathered data over the resrService

        if (await this.service.addResident(data)){    
            Materialize.toast(`bewoner: ${data.firstName} ${data.lastName} succesvol toegevoegd`, 5000);
        }else{
            Materialize.toast(`niet gelukt lan`, 5000);
        }


        // Reset Residents form

        form.reset();

        //close modal/form and 'reload' page 
        $("#add-resident-modal").modal("close");
        this.showAllResidents();

    }
    /**
     * Reset the form on close
     * @param form of type NgForm
     */
    resetForm(form: NgForm){form.reset();}

    openResidentAddModal(){
        $("#add-resident-modal").modal();
        $("#add-resident-modal").modal("open");
        $('.datepicker').pickadate({
            selectMonths: true, // Creates a dropdown to control month
            selectYears: 110, // Creates a dropdown of 15 years to control year,
            max: new Date(),
            monthsFull: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'December'],
            monthsShort: ['Jan','Feb','Maa','Apr','Mei','Jun','Jul','Aug','Sep','Okt','Nov','Dec'],
            weekdaysShort: ['ma', 'di', 'wo', 'do', 'vr', 'zat', 'zon'],
            today: 'Vandaag',
            clear: 'Wissen',
            close: 'Ok',
            formatSubmit: 'mm-dd-yyyy',
            dateFormat: 'mm-dd-yyyy',
            format: 'mm-dd-yyyy', //hier loopt iets mis?
            hiddenName: true,
            closeOnSelect: true // Close upon selecting a date,
        });

    }

    /**
     * Navigate to deffrent page with Object resident (only ID is enough) --> this will set the URL for that resident which makes it possible to Get said resident
     * @param resident of type Resident
     */
    navigateTo(resident: Resident) {
        console.log(resident.id);
        this.router.navigate(['/resident/' + resident.id]);
    }
        

}

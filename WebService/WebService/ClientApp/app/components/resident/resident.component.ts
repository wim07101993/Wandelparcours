import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'
import { Ng2SearchPipeModule } from 'ng2-search-filter'
import { async } from '@angular/core/testing';
declare var $:any;

@Component({
  selector: 'app-resident',
  templateUrl: './resident.component.html',
  styleUrls: ['./resident.component.css'],
  providers: [RestServiceService]
})
export class ResidentComponent implements OnInit {

    view: string = "card-view";
    data: any = null;
    residents: Resident[];
    modalResident: Resident;
    updateResident: any;
    search: boolean = false;
    


    constructor(private service: RestServiceService) {
        this.showAllResidents();
        this.residents = [];
        this.modalResident = <Resident>{
            firstName: "", lastName: "", room: "", id: "", birthday: new Date(), doctor: { name: "", phoneNumber: "" }
        };
        this.updateResident = {
            firstName: "", lastName: "", room: "", id: "", birthday: "", doctor: { name: "", phoneNumber: "" }
        };


    } 

    focusInput() {
        let a: any;
        setTimeout(() => {
            $('#test').focus();
        }, 200);  
        
    }

    /**
     * 
     * @param modalResident
     * Open Modal 
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
                today: 'Today',
                clear: 'Clear',
                close: 'Ok',
                closeOnSelect: false,
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
     * @param resident
     */
    async editResident(resident: Resident) {
        this.updateResident.id = resident.id;
        let birthDay = $("#birthDay").val();

        console.log(birthDay);
        
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
        for (let prop in this.updateResident)
        {
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

        let updateData = { value: this.updateResident, propertiesToUpdate: changedProperties };
        
        await this.service.editResidentWithData(updateData);

        this.updateResident = {
            firstName: "", lastName: "", room: "", id : "", birthday: "", doctor: { name: "", phoneNumber: "" }
        };

        this.showAllResidents();
    }

    openResidentAddModal(){
        $("#add-resident-modal").modal();
        $("#add-resident-modal").modal("open");
        
    }
  ngOnInit() {
  }



}

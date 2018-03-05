import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'
import { Ng2SearchPipeModule } from 'ng2-search-filter'
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
    items: Resident;

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

    openModal(modalResident: Resident) {
        //alert(uniqueIdentifier);
        this.modalResident = modalResident;
        $("#deleteModalResident").modal();
        $("#deleteModalResident").modal("open");
    }

    openEditModal(modalResident: Resident) {
        this.modalResident = modalResident;
        //this.modalResident.birthday=new Date(modalResident.birthday)
        $("#editModalResident").modal();
        $("#editModalResident").modal("open");
        $('.datepicker').pickadate({
            selectMonths: true, // Creates a dropdown to control month
            selectYears: 200, // Creates a dropdown of 15 years to control year,
            monthsFull: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'December'],
            monthsShort: ['Jan','Feb','Maa','Apr','Mei','Jun','Jul','Aug','Sep','Okt','Nov','Dec'],
            weekdaysShort: ['maa', 'din', 'woe', 'don', 'vri', 'zat', 'zon'],
            today: 'Today',
            clear: 'Clear',
            close: 'Ok',
            formatSubmit: 'mm-dd-yyyy',
            dateFormat: 'mm-dd-yyyy',
            format: 'mm-dd-yyyy', //hier loopt iets mis?
            hiddenName: true,
            closeOnSelect: false // Close upon selecting a date,
        });
    }

    closeModal() {
        $().modal("close");
    }

    async showAllResidents() {
        let residents: any = await this.service.getAllResidents();
        this.items = residents;
        //for (let a of residents) {
            //testing.substring(0,testing.indexOf("T"))
          //  let b: string = "" + a.birthday;
            //let c = b.substring(0, b.indexOf("T"));
        //}


      if (residents != undefined)
          this.residents = residents;
      else {
          alert("oops! :( looks like something went wrong :(");
      }
    }

    async deleteResident(uniqueIdentifier: string) {
        await this.service.deleteResidentByUniqueId(uniqueIdentifier);
        this.showAllResidents();
    }

    async editResident(resident: Resident) {
        this.updateResident.id = resident.id;
        let birthDay = $("#birthDay").val();

        console.log(birthDay);
        if (birthDay != "") {
            //console.log("update birthday");
            let a = new Date(birthDay);
            //let b = a.toLocaleDateString();
            //console.log(b)
            console.log(a);
            //this.updateResident.birthday = b;
            this.updateResident.birthday = a;
        }

        //console.log(this.updateResident);
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
        console.log(changedProperties);

        $('#birthDay').val("");

        //updatedResident.firstName = 
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

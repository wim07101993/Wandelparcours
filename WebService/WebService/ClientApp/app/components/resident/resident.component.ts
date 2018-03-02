import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'
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
    
    constructor(private service: RestServiceService) {
        this.showAllResidents();
        this.residents = [];
        this.modalResident = <Resident>{
            firstName: "", lastName: "",room:"", id: "", birthday: new Date(), doctor: { name: "", phoneNumber: "" }
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
        $("#editModalResident").modal();
        $("#editModalResident").modal("open");
        $('.datepicker').pickadate({
            selectMonths: true, // Creates a dropdown to control month
            selectYears: 200, // Creates a dropdown of 15 years to control year,
            today: 'Today',
            clear: 'Clear',
            close: 'Ok',
            closeOnSelect: false // Close upon selecting a date,
        });

    }

    closeModal() {
        $().modal("close");
    }

    async showAllResidents() {
        let residents = await this.service.getAllResidents();
    
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

  ngOnInit() {
  }



}

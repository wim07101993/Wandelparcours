import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'
declare var $: any

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
    constructor(private service: RestServiceService) {
        this.showAllResidents();
        this.residents = [];

    }

    openModal(uniqueIdentifier: string) {
        //alert(uniqueIdentifier);
        $("#modal" + uniqueIdentifier).modal();
        $("#modal" + uniqueIdentifier).modal("open");
    }

    openEditModal(uniqueIdentifier: string) {
        $("#modalEdit" + uniqueIdentifier).modal();
        $("#modalEdit" + uniqueIdentifier).modal("open");
    }

    closeModal() {
        $().modal("close");
    }

    async showAllResidents() {
      let residents= await this.service.getAllResidents();
      if (residents != undefined)
          this.residents = residents;
      else {
          alert("oops! :( looks like something went wrong :(");
      }
    }

  ngOnInit() {
  }

}

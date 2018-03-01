import { Component, OnInit } from '@angular/core';
import { Resident } from '../../models/resident';
declare var $:any;

@Component({
  selector: 'app-resident',
  templateUrl: './resident.component.html',
  styleUrls: ['./resident.component.css']
})
export class ResidentComponent implements OnInit {
    public model: Resident;
    constructor() {
        this.model = new Resident();
        this.model.param1 = "Test";
    }

  ngOnInit() {
  }

  openModal(){
    $('.modal').modal();
    $('.modal').modal('open');
    $('.datepicker').pickadate({
                selectMonths: true,
                selectYears: 200,
                today: 'Vandaag',
                clear: 'Wis',
                close: 'Akkoord',
                closeOnSelect: false
            });
  }


}

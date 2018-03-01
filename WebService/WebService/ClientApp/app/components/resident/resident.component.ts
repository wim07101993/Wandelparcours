import { Component, OnInit } from '@angular/core';
import { Resident } from '../../models/resident';

@Component({
  selector: 'app-resident',
  templateUrl: './resident.component.html',
  styleUrls: ['./resident.component.css']
})
export class ResidentComponent implements OnInit {
    public model: Resident;
    constructor() {
        this.model = new Resident();
        this.model.param1 = "Test"
    }

  ngOnInit() {
  }

}

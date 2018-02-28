import { Component, OnInit } from '@angular/core'
import { Resident } from '../../models/resident'
import { RestServiceService } from '../../service/rest-service.service' 
import { Response } from '@angular/http'

@Component({
  selector: 'app-resident',
  templateUrl: './resident.component.html',
  styleUrls: ['./resident.component.css'],
  providers: [RestServiceService]
})
export class ResidentComponent implements OnInit {
    public model: Resident;
    data: any = null;

    constructor(private service: RestServiceService) {
        this.model = new Resident();
        console.log(this.bla());
        this.testData();
    }

    bla() {
        return this.service.testData();
    }


   testData() {
        return this.service.getAllResidents().subscribe(data => {
            this.data = data;
            console.log(this.data);

        });
    }

  ngOnInit() {
  }

}

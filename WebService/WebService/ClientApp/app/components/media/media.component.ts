import { Component, OnInit } from '@angular/core';
import { Http, RequestOptions, URLSearchParams } from '@angular/http';
import { RestServiceService } from '../../service/rest-service.service';
import { Resident } from '../../models/resident';
import { async } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-media',
  templateUrl: './media.component.html',
  styleUrls: ['./media.component.css'],
  providers:[RestServiceService]
})
export class MediaComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    ngOnInit() {
        //this.id = this.route.snapshot.params['id'];
        //console.log(this.id)
    }

    resident: Resident;

    constructor(private service: RestServiceService, private route: ActivatedRoute) {
        this.showOneResident();
        this.resident = <Resident>{ firstName:"", lastName: "", room: "", id: "", birthday: new Date(), doctor: { name: "", phoneNumber: "" }};      
    }

    async showOneResident() {
        let resident: any = await this.service.getResidentBasedOnId(this.id);
        console.log(resident);
        if (resident != undefined)
           this.resident = resident;
        else {
            console.log("oops! :( looks like something went wrong :(");
        }
    }
  

}

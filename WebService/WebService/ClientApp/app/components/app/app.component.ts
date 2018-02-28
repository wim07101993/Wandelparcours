import { Component } from '@angular/core';
import { RestServiceService } from '../../service/rest-service.service';
//import { RestServiceService } from '../../service/rest-service.service';


@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    providers: [RestServiceService]
})
export class AppComponent {
    constructor(public restService: RestServiceService) {}
}

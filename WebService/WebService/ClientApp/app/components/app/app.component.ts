import { Component, ErrorHandler } from '@angular/core';
import { RestServiceService } from '../../service/rest-service.service';
import { CustomErrorHandler } from '../../service/customErrorHandler';


@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    providers: [RestServiceService, CustomErrorHandler]
})
export class AppComponent {
    //constructor(public restService: RestServiceService) {}
}

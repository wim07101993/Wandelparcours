import { Component, ErrorHandler } from '@angular/core';
import { RestServiceService } from '../../service/rest-service.service';
import { CustomErrorHandler } from '../../service/customErrorHandler';
import { MediaService } from '../../service/media.service';
declare var $:any;


@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    providers: [RestServiceService, CustomErrorHandler, MediaService]
})
export class AppComponent {
    //constructor(public restService: RestServiceService) {}
}

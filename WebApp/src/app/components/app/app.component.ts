import {Component} from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {MediaService} from '../../service/media.service';

declare var $: any;


@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [RestServiceService, MediaService]
})
export class AppComponent {
}

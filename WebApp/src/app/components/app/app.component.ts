import {Component} from '@angular/core';
import {RestServiceService} from '../../service/rest-service.service';
import {CustomErrorHandler} from '../../service/customErrorHandler';
import {MediaService} from '../../service/media.service';
import { Router } from '@angular/router';
import { LoginService } from "../../service/login-service.service";
declare var $: any;


@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [RestServiceService, CustomErrorHandler, MediaService]
})
export class AppComponent {
  constructor(private router:Router,private loginService:LoginService){
    this.loginService.checkLogin();
  }
}

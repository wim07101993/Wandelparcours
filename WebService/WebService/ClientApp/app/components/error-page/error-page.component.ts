import { Component, OnInit , OnDestroy} from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { CustomErrorHandler } from '../../service/customErrorHandler';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.css']
})
export class ErrorPageComponent implements OnInit {
    //errorMessage: string;
    errorMessage: string = '';
    subscription: Subscription;

    constructor(private route: ActivatedRoute, private customErrorHandler: CustomErrorHandler) {
        this.subscription = this.customErrorHandler.update$.subscribe(
            message => {
                this.errorMessage = message;
            });
    }

  ngOnInit() {
      //this.errorMessage = this.route.snapshot.data['message'];
      //this.route.data.subscribe(
      //    (data: Data) => {
      //        this.errorMessage = data['message'];
      //    }
      //);
  }

  ngOnDestroy() {
      this.subscription.unsubscribe();
  }



}

import {Injectable} from '@angular/core';
import {Observable} from 'rxjs/Observable';
import {BehaviorSubject} from 'rxjs/BehaviorSubject';

@Injectable()
export class CustomErrorHandler {
  errorMessage: string;
  private updateSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');

  update$: Observable<string> = this.updateSubject.asObservable();

  updateMessage(message: any) {
    let a: string = "Status : " + message.status + " : " + message.statusText + " / ERRORMESSAGE: ";
    this.updateSubject.next(a + message["_body"]);
    console.log(message);
  }
}

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class CustomErrorHandler {
    errorMessage: string;
    private updateSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');

    update$: Observable<string> = this.updateSubject.asObservable();

    updateMessage(message: any) {
        this.updateSubject.next(message.status + " : " + message.statusText + "\n" + message["_body"]);
        console.log(message);
    }
}
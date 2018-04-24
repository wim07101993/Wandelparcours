import { Component } from '@angular/core';

@Component({
    selector: 'counter',
    styleUrls: ['./counter.component.css'],
    templateUrl: './counter.component.html'
    
})
export class CounterComponent {
    public currentCount = 0;

    public incrementCounter() {
        this.currentCount++;
    }
}

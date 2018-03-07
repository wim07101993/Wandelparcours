import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-senior',
  templateUrl: './senior.component.html',
  styleUrls: ['./senior.component.css']
})
export class SeniorComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    ngOnInit() { }

    constructor(private route: ActivatedRoute) {}
}

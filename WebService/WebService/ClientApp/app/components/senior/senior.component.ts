import {Component, OnInit, resolveForwardRef} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-senior',
  templateUrl: './senior.component.html',
  styleUrls: ['./senior.component.css']
})
export class SeniorComponent implements OnInit {
    id: string = this.route.snapshot.params['id'];
    type:string;

    ngOnInit() {
         this.route.url.subscribe(resolve =>{
             
             console.log(resolve.length);
             if (resolve.length == 2)
             {
                this.type = "personalia";
             }
             else {
                 this.type = resolve[resolve.length-1].path;
             }

             
             
         });
        //$('.tabs').tabs();

    }

    

    constructor(private route: ActivatedRoute) {}

    addActive(){
        $('.test').addClass('active');
    }
   
}

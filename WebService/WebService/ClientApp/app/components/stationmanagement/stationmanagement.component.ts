import {Component, ElementRef, Inject, Injectable, OnInit, ViewChild} from '@angular/core';
import { Http } from '@angular/http';
import {getBaseUrl} from "../../app.module.browser";
import Result = jasmine.Result;
import any = jasmine.any;
import {letProto} from "rxjs/operator/let";
import {Observable} from "rxjs/Observable";


@Component({
  selector: 'app-stationmanagement',
  templateUrl: './stationmanagement.component.html',
  styleUrls: ['./stationmanagement.component.css']
})


export class StationmanagementComponent implements OnInit {
    @ViewChild('myCanvas') canvasRef: ElementRef;
    canvas: any;
    image:any;
    constructor(private http: Http) {}

  async ngOnInit() {
      try{
        let canvas=(<HTMLCanvasElement>this.canvasRef.nativeElement);
        this.canvas=canvas.getContext("2d");
        //await this.loadImage();
        //this.drawImage();
      }catch (ex){
          console.log("error");
      }
  }
  
  
  async loadImage(){
        return new Promise(resolve => {
                let img = new Image();
                let parent= this;
                img.onload=function () {
                    parent.image=img;
                    resolve();
    
                };
                img.src=getBaseUrl()+"images/blueprint.jpg";      
            }
        );
      
    
  }
    
  async drawImage(){
        
    this.canvas.drawImage(this.image,0,0);
  }

}

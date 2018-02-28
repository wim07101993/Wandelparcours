import {Component, ElementRef, Inject, Injectable, OnInit, ViewChild} from '@angular/core';
import {Http, ResponseContentType} from '@angular/http';
import {getBaseUrl} from "../../app.module.browser";
import Result = jasmine.Result;
import any = jasmine.any;
import {letProto} from "rxjs/operator/let";
import {Observable} from "rxjs/Observable";
import {ThrowStmt} from "@angular/compiler";
import {MouseEvents, Point} from "./MouseEvents"

@Component({
  selector: 'app-stationmanagement',
  templateUrl: './stationmanagement.component.html',
  styleUrls: ['./stationmanagement.component.css']
})


export class StationmanagementComponent implements OnInit {
    @ViewChild('myCanvas') canvasRef: ElementRef;
    canvas:HTMLCanvasElement;
    context: CanvasRenderingContext2D;
    image:HTMLImageElement;
    position: Point;
    zoomFactor:number = 1;
    mouseEvents:MouseEvents;
    framerate=60;
    constructor(private http: Http) {}
    async ngOnInit() {
      try{
        this.canvas=(<HTMLCanvasElement>this.canvasRef.nativeElement);
        this.context=<CanvasRenderingContext2D> this.canvas.getContext("2d");
        console.log(this.context);
        let test =this.canvas.getContext("2d");
        await this.loadImage();
        this.mouseEvents= new MouseEvents(this);
        this.position={x:0,y:0};
        console.log(this.position);
        //setInterval(()=>{this.tick()},1000/this.framerate);
        this.tick();
      }catch (ex){
          console.log("error");
      }
    
    
    }
    async zoomIn(){
            
            this.zoomFactor*=2;
            
            
            
            this.tick();
    }
    async zoomOut(){

        this.zoomFactor/=2;
        
        this.tick();
    }
    async tick(){
        this.context.clearRect(0,0,this.canvas.width,this.canvas.height);
        
        this.drawMap();
    }     
  
  
  async loadImage(){
        return new Promise(resolve => {
                let image =new Image();
                let parent = this;
                image.onload=function () {
                    parent.image=image;
                    resolve();
                };
                image.src=getBaseUrl()+"images/blueprint.jpg";
                


            }
        );
      
    
  }
  
  async drawMap(){
        try{
            this.canvas.height=this.image.height/this.zoomFactor;
            this.canvas.width=this.image.width/this.zoomFactor;
            
            this.context.drawImage(this.image,this.mouseEvents.position.x,this.mouseEvents.position.y);
        }catch (ex){
            console.log(ex);
        }
  }

}

import {Component, ElementRef, Inject, Injectable, OnInit, ViewChild} from '@angular/core';
import {Http, ResponseContentType} from '@angular/http';
import {getBaseUrl} from "../../app.module.browser";
import Result = jasmine.Result;
import any = jasmine.any;
import {letProto} from "rxjs/operator/let";
import {Observable} from "rxjs/Observable";
import {ThrowStmt} from "@angular/compiler";
import {MouseEvents, Point} from "./MouseEvents"
import {RenderBuffer,bufferelement} from "./RenderBuffer"
import {Station} from "../../models/station"
declare var $: any
declare var Materialize:any
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
    marker:HTMLImageElement;
    position: Point;
    renderBuffer:RenderBuffer;
    zoomFactor:number = 1;
    mouseEvents:MouseEvents;
    framerate=5;
    imageUrl="";
    markerUrl="";
    adMarker:boolean=false;
    
    saveStation:Station;
    
    stations:Station[]=[];
    markersize=25;
    async saveNewStation(){
        let reg=new RegExp("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        let mac =this.saveStation.mac;
        console.log(mac);
        if (reg.test(mac)){
            await this.saveStationToDatabase(this.saveStation);
            this.stations = await this.loadStations();
            this.saveStation=new Station();
            $("#markerModel").modal("close");
            this.adMarker=false;
        }else{
            Materialize.toast('Station adres verkeerd', 4000);
        }
        
    }
   
    constructor(private http: Http) {
        this.saveStation=new Station();
    }
    async ngOnInit() {
        
      try{
          
            //load all available stations from db
            this.stations = await this.loadStations();
            //put the rendering canvas in a variable to draw
            this.canvas=(<HTMLCanvasElement>this.canvasRef.nativeElement);
            this.context=<CanvasRenderingContext2D> this.canvas.getContext("2d");
            //set the imageurl
            this.imageUrl= getBaseUrl()+"images/blueprint.jpg";
            this.markerUrl=getBaseUrl()+"images/station.png";
            //create class mouseevent(this class is responsible for all mouse events in the render canvas)
            this.mouseEvents= new MouseEvents(this);
            //create render buffer
            this.renderBuffer=new RenderBuffer(this);
            //load the blueprint of the building
            await this.loadMap();
            //load marker
            await this.loadMarker();
            //set the position of the map on the canvas
            this.position={x:0,y:0};
            $("#markerModel").modal();
            
            
            //set a auto render of 5 fps
            setInterval(()=>{this.tick()},1000/this.framerate);
          this.tick();
            
      }catch (ex){
          console.log("error");
      }
    
    
    }

    
    async closeModal(){
        $("#markerModel").modal("close");
    }
    
    //this function is needed to zoomin
    async zoomIn(){
            
            this.zoomFactor*=2;
            
            
            
            this.tick();
    }
    //this function is needed to zoomout
    async zoomOut(){

        this.zoomFactor/=2;
        
        this.tick();
    }
    //tick does the needed calculatations for the render, and draws the rendering on the canvas
    async tick(){
        try{
            await this.renderBuffer.clear();
            await this.drawMap();
            await this.drawModules();
            await this.drawStationOnCursor();
            await this.renderBuffer.render();
        }catch (ex){console.log(ex);}
    }     
  
    async drawStationOnCursor(){
        if (this.adMarker){
            let renderBuffer:RenderBuffer=this.renderBuffer;
            let image:HTMLImageElement=this.marker;
            
            let width=this.width/this.markersize;
            let x =this.mouseEvents.mousepos.x-(width/2);
            let y =this.mouseEvents.mousepos.y-(width);
            await renderBuffer.add(this.marker,x, y,width,width,"marker","marker");
            

            
        }
    }
    async saveStationToDatabaseModal(stationPosition:Point){
        this.saveStation.position.x=stationPosition.x;
        this.saveStation.position.y=stationPosition.y;
        $("#markerModel").modal("open");
    }
  //this function loads the image of the building
  async loadMap(){
        return new Promise(resolve => {
                let image =new Image();
                let parent = this;
                image.onload=function () {
                    parent.image=image;
                    //return the promise
                    resolve();
                };
                //trigger onload for the imagurl
                image.src=this.imageUrl;
                


            }
        );
      
    
  }
  async loadMarker(){
      return new Promise(resolve => {
              let image =new Image();
              let parent = this;
              image.onload=function () {
                  parent.marker=image;
                  //return the promise
                  resolve();
              };
              //trigger onload for the imagurl
              image.src=this.markerUrl;



          }
      );
  }
  //calculate width
  get width(){
        let width=0;
      if(window.innerHeight>window.innerWidth){
          
          width = window.innerHeight/this.image.height*this.image.width;
      }else{
          width = window.innerWidth;
      }
      return width;
  }
  //calculate height
  get height(){
        let height=0;
      if(window.innerHeight>window.innerWidth){
          height=window.innerHeight;
          
      }else{
          height= window.innerWidth/ this.image.width*this.image.height;
      }
      return height;
  }
  
  async drawMap(){
        try{
            
            this.canvas.height=window.innerHeight;
            this.canvas.width=window.innerWidth;
      
            //calculate zoom
            let height=this.height*this.zoomFactor;
            let width=this.width*this.zoomFactor;
            //render
            this.renderBuffer.add(this.image,this.mouseEvents.position.x, this.mouseEvents.position.y,width,height,"map","map");
            
            //add the map location/size to the mouseevents for relative location calculation
            this.mouseEvents.mapPos={x:this.mouseEvents.position.x,y:this.mouseEvents.position.y, width:width,height:height};
            
        }catch (ex){
            console.log(ex);
        }
  }
  
  
  async drawModules(){
        for(let station of this.stations){
          let renderBuffer:RenderBuffer=this.renderBuffer;
          let image:HTMLImageElement=this.marker;
          let width=this.width/this.markersize;
          
          let position= this.mouseEvents.calculateStationPosOnImage(station.position);
          let x =position.x-(width/2);
          let y =position.y-width;
            
          await renderBuffer.add(this.marker,x, y,width,width, station.mac ,"marker");
        }
        
  }




    async saveStationToDatabase(station:Station){
        console.log(JSON.stringify(station));
        return new Promise(resolve => {

            this.http.post("http://localhost:5000/api/v1/receivermodules",station).subscribe(response => {
                    try{
                        resolve("success");
                    }catch (e){
                        resolve("error");
                    }

                },
                error =>{
                    resolve("error");
                }
            )
        });
    }




async loadStations(){
        return new Promise<Station[]>(resolve => {
            
           this.http.get("http://localhost:5000/api/v1/receivermodules").subscribe(response => {
               try{
                   let tryParse=response.json();
                   resolve(<Station[]>tryParse);
               }catch (e){
                   resolve([]);
               }
               
           },
            error =>{
               resolve([]);
            }
        )
        });
  }

}

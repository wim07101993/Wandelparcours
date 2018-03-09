import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Http} from '@angular/http';
import {getBaseUrl} from "../../app.module.browser";
import {MouseEvents, Point} from "./MouseEvents"
import {Renderer} from "./Renderer"
import {RenderBuffer,} from "./RenderBuffer"
import {Station} from "../../models/station"
import {Sprites} from "./Sprites"
declare var $: any;
declare var Materialize:any;
@Component({
  selector: 'app-stationmanagement',
  templateUrl: './stationmanagement.component.html',
  styleUrls: ['./stationmanagement.component.css']
})



export class StationmanagementComponent implements OnInit {
    @ViewChild('myCanvas') canvasRef: ElementRef;
    
    position: Point;
    renderBuffer:RenderBuffer;
    zoomFactor:number = 1;
    mouseEvents:MouseEvents;
    framerate=5;
    imageUrl="";
    markerUrl="";
    adMarker:boolean=false;
    collidingElement:any;
    saveStation:Station=new Station();
    menu:boolean=false;
    stations=new Map<string,Point>();
    stationMacAdresses:string[]=[];
    markerscale=25;
    renderer:Renderer;
    markersize: number;
    async saveNewStation(){
        let reg=new RegExp("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        let mac =this.saveStation.mac;
        console.log(mac);
        if (reg.test(mac)){
            
            await this.saveStationToDatabase(this.saveStation);
            await this.loadStations();
            this.saveStation=new Station();
            $("#markerModel").modal("close");
            this.adMarker=false;
        }else{
            Materialize.toast('Station adres verkeerd', 4000);
        }
        
    }
   
    constructor(private http: Http) {}

    async ngOnInit() {
        //create renderer
        this.renderer = new Renderer(this);
        //set the imageurl
        this.imageUrl= getBaseUrl()+"images/blueprint.jpg";
        this.markerUrl=getBaseUrl()+"images/station.png";
        this.renderBuffer=new RenderBuffer(this);
        this.mouseEvents= new MouseEvents(this);
        //load the blueprint of the building
        await this.loadMap();
        await this.DownloadMarker();
        await this.loadStations();
        await this.renderer.CleanAndUpdateRenderBuffer();
        //load marker
       
        setInterval(()=>{this.tick()},1000/this.framerate);
        /* try{
              //load all available stations from db
             this.stations = await this.loadStations();
              //put the rendering canvas in a variable to draw 
              this.canvas=(<HTMLCanvasElement>this.canvasRef.nativeElement);
              
              //get drawing context of canvas as 2d
              this.context=<WebGLRenderingContext> this.canvas.getContext("webgl");
              
              //create class mouseevent(this class is responsible for all mouse events in the render canvas)
              //create render buffer
              
              //set the position of the map on the canvas
              this.position={x:0,y:0};
              $("#markerModel").modal();
              
              
              //set a auto render of 5 fps
            this.tick();
              
        }catch (ex){
            console.log("error");
        }*/
    
    
    }

    
    static async closeModal(){
        $("#markerModel").modal("close");
    }
    
    async deleteModal(id?:string){
        
        if (id!=undefined){
            this.collidingElement =  id;
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal();
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal("open");
        }
          
        
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
            //await this.renderBuffer.clear();
            await this.renderer.fixCanvas();
            await this.RecalculateMap();
            await this.RecalculateStations();
            //await this.drawModules();
            await this.drawStationOnCursor();
            
            //await this.renderBuffer.render();
        }catch (ex){console.log(ex);}
    }     
  
    async drawStationOnCursor(){
        if (this.adMarker){
            let width=this.width/this.markerscale;
            this.markersize= width;
            let x =this.mouseEvents.mousepos.x-(width/2);
            let y =this.mouseEvents.mousepos.y-(width);
            let station=this.renderBuffer.cursorStation;
            station.x = x;
            station.y = y;
            station.width=width;
            station.height=width;
        
            

            
        }else{
            let station=this.renderBuffer.cursorStation;
            station.x = -9999999999999;
            station.y = -9999999999999;
            station.width=0;
            station.height=0;
        }
        
    }
    async saveStationToDatabaseModal(stationPosition:Point){
        this.saveStation.position.x=stationPosition.x;
        this.saveStation.position.y=stationPosition.y;
        $("#markerModel").modal();
        $("#markerModel").modal("open");
    }
  //this function loads the image of the building
  async loadMap(){
        //download the map
      await this.renderer.LoadImages(this.imageUrl,Sprites.map);
      //put it in a sprite
      this.renderBuffer.map=await this.renderer.createSprite(Sprites.map);
      //adjust height and width
      this.renderBuffer.map.width=this.width;
      this.renderBuffer.map.height=this.height;
      
      return;
      
    
  }
  //load marker;
  async DownloadMarker(){
        await this.renderer.LoadImages(this.markerUrl,"marker");
        this.renderBuffer.cursorStation=this.renderer.createSprite(Sprites.marker);
        this.renderBuffer.cursorStation.width=0;
      this.renderBuffer.cursorStation.height=0;
      this.renderBuffer.cursorStation.x=-99999;
      this.renderBuffer.cursorStation.y=-99999;
        console.log(this.renderBuffer.cursorStation);
        return;
  }
  //calculate width
  get width(){
        let width=1;
        let map = this.renderer.createSprite(Sprites.map);
        
        if (map==undefined)return 0;
        if(window.innerHeight>window.innerWidth){      
          //todo uncomment
            width = window.innerHeight/map.height*map.width;
        }else{
          width = window.innerWidth;
        }
        return width;
  }
  //calculate height
  get height(){
        let height=0;
      let map = this.renderer.createSprite(Sprites.map);
      if (map==undefined)return 0;
      if(window.innerHeight>window.innerWidth){
          height=window.innerHeight;
          
      }else{
          //todo uncomment
          height= window.innerWidth/ map.width*map.height;
      }
      return height;
  }
  
  async RecalculateMap(){
        try{
            
            this.canvasRef.nativeElement.height=window.innerHeight;
            this.canvasRef.nativeElement.width=window.innerWidth;
      
            //calculate zoom
            let height=this.height*this.zoomFactor;
            let width=this.width*this.zoomFactor;
            //render
            let map = this.renderBuffer.map;
            map.width=width;
            map.height=height;
            map.x = this.mouseEvents.position.x;
            map.y = this.mouseEvents.position.y;
            //todo render with sprite
            //this.renderBuffer.add(this.image,this.mouseEvents.position.x, this.mouseEvents.position.y,width,height,"map","map");
            
            //add the map location/size to the mouseevents for relative location calculation
            this.mouseEvents.mapPos={x:this.mouseEvents.position.x,y:this.mouseEvents.position.y, width:width,height:height};
            
        }catch (ex){
            console.log(ex);
        }
  }
  
  
  async RecalculateStations(){
        let refreshNeeded=false;
        this.stations.forEach((location:any,key:any,map:any)=>{
            
            let renderBuffer:RenderBuffer=this.renderBuffer;
            let width=this.width/this.markerscale;
            this.markersize=width;
            let position= this.mouseEvents.calculateStationPosOnImage(location);
            let x =position.x-(width/2);
            let y =position.y-width;
            let station = this.renderBuffer.buffer.get(key);
            if (station ==undefined){
                station=this.renderBuffer.AddSpriteToBufferById(key, Sprites.marker);
                refreshNeeded=true;
            }
            if (station!=undefined){
                station.x = x;
                station.y = y;
                station.width=width;
                station.height=width;
            }
            
        });
        if (refreshNeeded){
            this.renderer.CleanAndUpdateRenderBuffer();
        }
        /*
        for(let station of this.stations){
          
          
      
            //todo render with buffer
          //await renderBuffer.add(this.marker,x, y,width,width, station.mac ,"marker");
        }*/
        
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

    async deleteStation(mac:string){
        return new Promise(resolve => {

            this.http.delete("http://localhost:5000/api/v1/receivermodules/"+mac).subscribe(response => {
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
    try{
        this.stations.clear();
        this.renderBuffer.buffer.clear();
        this.stationMacAdresses=[];
        return new Promise(resolve => {
            
           this.http.get("http://localhost:5000/api/v1/receivermodules").subscribe(response => {
             
                   let tryParse=<Array<Station>>(response.json());
                   let station:Station;
                   for (station of tryParse){
                       this.stationMacAdresses.push(station.mac);
                       this.stations.set(station.mac,station.position);
                       
                       
                   }
                   resolve(true);
             
           },
            error =>{
               resolve(true);
            }
        )
        });
    }catch (e){
        console.log("can't load image");
    }

}

    async deleteCurrentStation() {
        await this.deleteStation(this.collidingElement);
        await this.loadStations();
        $("#deleteModal").modal("close");
    }
}

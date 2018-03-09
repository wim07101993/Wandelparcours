import {StationmanagementComponent} from "./stationmanagement.component"
import {getBaseUrl} from "../../app.module.browser";
import {bufferelement, RenderBuffer} from "./RenderBuffer";
import {Position} from "../../models/station"
declare var $: any;
declare var Hammer:any;
export class MouseEvents{
    station:StationmanagementComponent;
    canvas : HTMLCanvasElement;
    clicked:boolean=false;
    
    //position of the mouse on the last frame, for comparison in pan
    position:Point={x:0,y:0};
    
    
    //position of the map
    mapPos:any={x:0,y:0,width:0,height:0};
    
    
    
    //the position of pan last frame
    panLastPos:Point={x:0,y:0};
    x=0;
    zoomfactor=1;
    //position of the mouse relative to the map
    mousepos:any;
    constructor(station:StationmanagementComponent){
        //load in variables from the stationmanagement component
        this.station=station;
        this.canvas=this.station.canvasRef.nativeElement;
        //ad listeners
        this.canvas.addEventListener("mousemove",(ev => this.MouseMove(ev)));
        this.canvas.addEventListener("contextmenu",ev =>  MouseEvents.RightClick(ev));
        this.canvas.addEventListener("mousedown", ev =>  this.MouseDown(ev));
        this.canvas.addEventListener('mousewheel',function(event){    return false;}, false);


        //instantiate panner;
        let hammer = new Hammer(this.canvas);
        let parent = this;
        //add listener for panner
        hammer.on("panmove",function (e:any){MouseEvents.Panning(e,parent);});
        hammer.on("panend",function(){MouseEvents.PanStop(parent);});
        
        
        //hammer
    }

    static async PanStop( parent:MouseEvents){
        parent.panLastPos={x:0,y:0};
    }
    static async Panning(e:any, parent:MouseEvents){
        
        
        
        
        parent.position={
            x:parent.position.x+(e.deltaX-parent.panLastPos.x),
            y:parent.position.y+(e.deltaY-parent.panLastPos.y)
        };
        
        parent.panLastPos.x=e.deltaX;
        parent.panLastPos.y=e.deltaY;
        await parent.station.Tick();
        
    }
    

    async MouseMove(e:MouseEvent){
        
        this.mousepos={x:e.x,y:e.y};
        if (this.station.adMarker){
            await this.station.Tick();
        }
        
        
    }
    
    

    async CalculateMousePosOnImage(mousePos:Point){
        let x=(mousePos.x-this.mapPos.x)/this.mapPos.width;
        let y =(mousePos.y-this.mapPos.y)/this.mapPos.height;
        return {x: x, y: y};
    }

     CalculateStationPosOnImage(position:Point){
         return {
            x: (position.x * this.station.renderBuffer.map.width) + this.mapPos.x,
            y: (position.y * this.station.renderBuffer.map.height) + this.mapPos.y
        };
        
    }
    
  
    async MouseDown(e:MouseEvent){
        if (this.station.adMarker){
            let mouseposition = await this.CalculateMousePosOnImage({x:e.x,y:e.y});
            this.station.SaveStationToDatabaseModal(mouseposition)
            
        }else{
            this.mousepos={x:e.x,y:e.y};
        }
    }
    
    static async RightClick(e:MouseEvent){
        //prevent right clicks on the map
        e.preventDefault();
    }
    
    
}

export interface Point{
    x:number;
    y:number;
}


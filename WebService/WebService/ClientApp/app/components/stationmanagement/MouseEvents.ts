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
        this.canvas.addEventListener("mousemove",(ev => this.mouseMove(ev)));
        this.canvas.addEventListener("contextmenu",ev =>  this.rightClick(ev));
        this.canvas.addEventListener("mousedown", ev =>  this.mouseDown(ev));
        this.canvas.addEventListener("mouseup",ev => this.mouseUp(ev));

        this.canvas.addEventListener('mousewheel',function(event){    return false;}, false);


        //instantiate panner;
        let hammer = new Hammer(this.canvas);
        let parent = this;
        hammer.on("panmove",function (e:any){MouseEvents.panning(e,parent);});
        hammer.on("panend",function(){MouseEvents.panStop(parent);});
        
        
        //hammer
    }

    static async panStop( parent:MouseEvents){
        parent.panLastPos={x:0,y:0};
    }
    static async panning(e:any, parent:MouseEvents){
        
        
        
        
        parent.position={
            x:parent.position.x+(e.deltaX-parent.panLastPos.x),
            y:parent.position.y+(e.deltaY-parent.panLastPos.y)
        };
        
        parent.panLastPos.x=e.deltaX;
        parent.panLastPos.y=e.deltaY;
        parent.station.tick();
        
    }
    

    async mouseMove(e:MouseEvent){
        
        this.mousepos={x:e.x,y:e.y};
        if (this.station.adMarker){
            this.station.tick();
        }
        
        
    }
    
    

    async calculateMousePosOnImage(mousePos:Point){
        let x=(mousePos.x-this.mapPos.x)/this.mapPos.width;
        let y =(mousePos.y-this.mapPos.y)/this.mapPos.height;
        return {x: x, y: y};
    }

     calculateStationPosOnImage(position:Point){
         return {
            x: (position.x * this.station.renderBuffer.map.width) + this.mapPos.x,
            y: (position.y * this.station.renderBuffer.map.height) + this.mapPos.y
        };
        
    }
    
    async mouseUp(e:MouseEvent){
        //disable tracking
        //todo migrate to pan
        //this.clicked=false;
        
        
    }
    async mouseDown(e:MouseEvent){
        if (this.station.adMarker){
            let mouseposition = await this.calculateMousePosOnImage({x:e.x,y:e.y});
            this.station.saveStationToDatabaseModal(mouseposition)
            
        }else{
            this.mousepos={x:e.x,y:e.y};
        }
    }
    
    async rightClick(e:MouseEvent){
        //prevent right clicks on the map
        e.preventDefault();
    }
    
    
}

export interface Point{
    x:number;
    y:number;
}


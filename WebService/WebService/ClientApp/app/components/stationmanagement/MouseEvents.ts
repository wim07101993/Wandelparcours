import {StationmanagementComponent} from "./stationmanagement.component"
import {getBaseUrl} from "../../app.module.browser";

export class MouseEvents{
    station:StationmanagementComponent;
    canvas : HTMLCanvasElement;
    clicked:boolean=false;
    position:Point;
    screenPos:Point;
    x=0;
    zoomfactor=1;
    constructor(station:StationmanagementComponent){
        this.station=station;
        
        console.log(station);
        this.canvas=this.station.canvas;
        this.canvas.addEventListener("mousemove",(ev => this.mouseMove(ev)));
        this.canvas.addEventListener("contextmenu",ev =>  this.rightClick(ev));
        this.canvas.addEventListener("mousedown", ev =>  this.mouseDown(ev));
        this.canvas.addEventListener("mouseup",ev => this.mouseUp(ev));
        this.position={x:0,y:0};
        
        
        
    }
  

    async mouseMove(e:MouseEvent){
        if (this.clicked){
            this.position.x= this.position.x- (this.screenPos.x-e.screenX);
            this.position.y= this.position.y- (this.screenPos.y-e.screenY);
            this.screenPos = {x:e.screenX,y:e.screenY};
            this.station.tick();
            
            
        }
        
    }
    async mouseUp(e:MouseEvent){
        console.log(this.position);
        this.clicked=false;
        console.log("clikied");
        
    }
    async mouseDown(e:MouseEvent){
        
        this.clicked=true;
        
        this.screenPos = {x:e.screenX,y:e.screenY};
        console.log("cliking");
    }
    
    async rightClick(e:MouseEvent){
        e.preventDefault();
    }
    
    
}

export interface Point{
    x:number;
    y:number;
}


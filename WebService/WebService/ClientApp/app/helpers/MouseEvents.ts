import {ARenderComponent} from "./ARenderComponent"
import {bufferelement, RenderBuffer} from "./RenderBuffer";
import {Position} from "../models/station"
declare var $: any;
declare var Hammer:any;
/** Class to track mouse events */
export class MouseEvents{
    aRenderComponent:ARenderComponent;
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

    /**
     * Creating MouseEvent object.
     * @param aRenderComponent
     */
    constructor(aRenderComponent:ARenderComponent){
        //load in variables from the stationmanagement component
        this.aRenderComponent=aRenderComponent;
        this.canvas=this.aRenderComponent.canvasRef.nativeElement;
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
    /*
    * Stop panning
    *  @param {MouseEvents} parent - mouseevents object is passed trough param because (this) is not available in a es2015 event
    */
    static async PanStop( parent:MouseEvents){
        parent.panLastPos={x:0,y:0};
    }

    /*
    * Panning the canvas
    *  @param {any} e- this is the event that happened on the canvas
    *  @param {MouseEvents} parent - mouseevents object is passed trough param because (this) is not available in a es2015 event
    */
    static async Panning(e:any, parent:MouseEvents){
        
        
        
        
        parent.position={
            x:parent.position.x+(e.deltaX-parent.panLastPos.x),
            y:parent.position.y+(e.deltaY-parent.panLastPos.y)
        };
        
        parent.panLastPos.x=e.deltaX;
        parent.panLastPos.y=e.deltaY;
        await parent.aRenderComponent.Tick();
        
    }

    /*
    *  This function tracks the movement of the mouse on the canves (used to put a station on the cursor)
    *  @param {any} e- this is the event that happened on the canvas
    */
    async MouseMove(e:MouseEvent){
        
        this.mousepos={x:e.x,y:e.y};
        if (this.aRenderComponent.adMarker){
            await this.aRenderComponent.Tick();
        }
        
        
    }
    
    /*
    *   This function calulates the position of the mouse on the image
    *   @param {Point} mousePos - this is the position of the mouse on the canvas
    *   @return {Point} The position of the mouse relative to the image
    */

    async CalculateMousePosOnImage(mousePos:Point){
        let x=(mousePos.x-this.mapPos.x)/this.mapPos.width;
        let y =(mousePos.y-this.mapPos.y)/this.mapPos.height;
        return {x: x, y: y};
    }
    /*
    * This function calculates the position of te station on the image
    * @param  {Point} this is the location of the station on the image in %, 0,0 being left bottom corner and 1,1 being right top corner
    * @return {Point} this is the location of the station on the canvas on wich it is drawn on.
    */
     CalculateStationPosOnImage(position:Point){
         return {
            x: (position.x * this.aRenderComponent.renderBuffer.map.width) + this.mapPos.x,
            y: (position.y * this.aRenderComponent.renderBuffer.map.height) + this.mapPos.y
        };
        
    }

    /*
     * This function is triggered when click on the mouse, is called when the user wants to put a station on the map
     * @param  {MouseEvent} e - event that the mouse has done  
     */
    async MouseDown(e:MouseEvent){
        if (this.aRenderComponent.adMarker){
            let mouseposition = await this.CalculateMousePosOnImage({x:e.x,y:e.y});
            this.aRenderComponent.SaveStationToDatabaseModal(mouseposition)
            
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


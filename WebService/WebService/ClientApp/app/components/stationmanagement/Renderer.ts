import {StationmanagementComponent} from "../stationmanagement/stationmanagement.component"
import * as PIXI from 'pixi.js'
import Sprite = PIXI.Sprite;
declare var window:any;


export class Renderer{
    app: PIXI.Application;
    station:StationmanagementComponent;
    images=new Map<string, string>();
    constructor(station:StationmanagementComponent){
        this.station=station;
       
            this.app=new PIXI.Application();
            (<HTMLDivElement>this.station.canvasRef.nativeElement).appendChild(this.app.view);
            this.tick();
    }
    CleanAndUpdateRenderBuffer(){
        this.clear();
        this.app.stage.addChild(this.station.renderBuffer.map);
        this.app.stage.addChild(this.station.renderBuffer.cursorStation);
        this.station.renderBuffer.buffer.forEach((sprite:Sprite,key:any,map:any)=>{
            this.app.stage.addChild(sprite);
            sprite.tint = 111111;
            sprite.interactive = true;
            sprite.buttonMode = true;
            sprite.on("pointerdown",(e:any)=>{
                this.station.deleteModal(key);
            });
            
        });
    }
    createSprite(key:string){
        let imageuri:string=<string>this.images.get(key);
        if (imageuri==null) throw "couldn't find image";
        return new Sprite(PIXI.loader.resources[imageuri].texture); 
    }
    
    async LoadImages(image:string, key:string){
        return new Promise(resolve => {
            try{
                PIXI.loader.add(image).load(()=>{
                    this.images.set(key,image);
                    resolve()
                });    
            }catch (er){
                
                resolve();
            }
        });
        
    }
    
    clear(){
        while (this.app.stage.children[0]){
            this.app.stage.removeChild(this.app.stage.children[0]);
        }
    }
    
    
    async fixCanvas(){
        let canvas=this.station.canvasRef.nativeElement;
        this.app.renderer.resize(canvas.offsetWidth,canvas.offsetHeight);
        this.app.renderer.autoResize=true;
        this.app.renderer.backgroundColor=0xffffff;
        return;
    }
    
    async tick(){
        await this.fixCanvas();
        
    }


}

import {StationmanagementComponent} from "./stationmanagement.component"


import * as PIXI from 'pixi.js'
import Sprite = PIXI.Sprite;
import {Sprites} from "./Sprites";


export class RenderBuffer{
    station:StationmanagementComponent;
    map:Sprite;
    cursorStation:Sprite;
    buffer=new Map<string, Sprite>();
    constructor(station:StationmanagementComponent){
        this.station=station;
     
    }


    

    AddSpriteToBufferById(mac:string,key:string){
        let sprite=this.station.renderer.CreateSprite(key);
        this.buffer.set(mac,sprite);
        return sprite;
    }

}
export interface bufferelement{
    image:HTMLImageElement;
    x:number; 
    y:number;
    width:number;
    height:number;
    id:string;
    className:string;
    
}
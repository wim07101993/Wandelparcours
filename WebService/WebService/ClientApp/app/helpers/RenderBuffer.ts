
import  {ARenderComponent} from "./ARenderComponent"

import * as PIXI from 'pixi.js'
import Sprite = PIXI.Sprite;
import {Sprites} from "./Sprites";

/** Class to buffer the elements wich need to get rendered */
export class RenderBuffer{
    aRenderComponent:ARenderComponent;
    map:Sprite;
    cursorStation:Sprite;
    buffer=new Map<string, Sprite>();

    /**
     * Creating RenderBuffer object.
     * @param aRenderComponent
     */
    constructor(aRenderComponent:ARenderComponent){
        this.aRenderComponent=aRenderComponent;
     
    }


    /**
     * Creating RenderBuffer object.
     * @param {string} id - this is the for which the object is uploaded
     * @param {string} key - this is the key to create a sprite
     * @return {Sprite} sprite- the sprite for this id alse gets returned back
     */
    AddSpriteToBufferById(id:string,key:string){
        let sprite=this.aRenderComponent.renderer.CreateSprite(key);
        this.buffer.set(id,sprite);
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
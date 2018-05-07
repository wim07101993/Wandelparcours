import {ARenderComponent} from "./ARenderComponent"

import * as PIXI from 'pixi.js'
import Sprite = PIXI.Sprite;

/** Class to buffer the elements wich need to get rendered */
export class RenderBuffer {
  aRenderComponent: ARenderComponent;
  map: Sprite;
  cursorStation: Sprite;
  buffer = new Map<string, any>();

  /**
   * Creating RenderBuffer object.
   * @param aRenderComponent
   */
  constructor(aRenderComponent: ARenderComponent) {
    this.aRenderComponent = aRenderComponent;
  }


  /**
   * Creating RenderBuffer object.
   * @param {string} id - this is the for which the object is uploaded
   * @param {string} key - this is the key to create a sprite
   * @return {Sprite} sprite- the sprite for this id alse gets returned back
   */
  AddSpriteToBufferById(id: string, key: string) {
    let sprite = this.aRenderComponent.renderer.CreateSprite(key);
    this.buffer.set(id, sprite);
    return sprite;
  }
  AddTextById(text){
    let style = new PIXI.TextStyle({
      fontFamily: 'Arial',
      fontSize: 90,
      fontWeight: 'bold',
      fill: ['#2a6496'], 
      stroke: '#444444',
      strokeThickness: 1,
      dropShadow: false,
      dropShadowColor: '#000000',
      dropShadowBlur: 5,
      dropShadowAngle: Math.PI / 6,
      dropShadowDistance: 5,
      wordWrap: true,
      wordWrapWidth: 440
    });
    let sprite =new PIXI.Text(text,style);
    this.buffer.set(text, sprite);
    return sprite;
  }
}

export interface bufferelement {
  image: HTMLImageElement;
  x: number;
  y: number;
  width: number;
  height: number;
  id: string;
  className: string;
}

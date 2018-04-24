import * as PIXI from 'pixi.js'
import {Sprites} from "./Sprites";
import Sprite = PIXI.Sprite;
import {ARenderComponent} from "./ARenderComponent";

declare var window: any;
declare var $: any;

export class Renderer {
  app: PIXI.Application;
  parentComponent: ARenderComponent;
  images = new Map<string, string>();


  get width() {
    let width = 1;
    let map = this.CreateSprite(Sprites.map);
    console.log();
    let parentWidth = $(this.parentComponent.hostElement.nativeElement.tagName).width();
    let parentHeight = $(this.parentComponent.hostElement.nativeElement.tagName).height();
    if (map == undefined) return 0;
    if (parentHeight > parentWidth) {
      width = parentHeight / map.height * map.width;
    } else {
      width = parentWidth;
    }
    return width;
  }

  /*
  *    Getter calculates relative the height of the image
  */
  get height() {
    let height = 0;
    let map = this.CreateSprite(Sprites.map);
    let parentWidth = $(this.parentComponent.hostElement.nativeElement.tagName).width();
    let parentHeight = $(this.parentComponent.hostElement.nativeElement.tagName).height();
    if (map == undefined) return 0;
    if (parentHeight > parentWidth) {
      height = parentHeight;
    } else {
      height = parentHeight / map.width * map.height;
    }
    return height;
  }

  constructor(parentComponent: ARenderComponent) {
    this.parentComponent = parentComponent;
    this.app = new PIXI.Application();
    (<HTMLDivElement>this.parentComponent.canvasRef.nativeElement).appendChild(this.app.view);
    this.Clear();
  }

  CleanAndUpdateRenderBuffer() {
    this.Clear();
    this.app.stage.addChild(this.parentComponent.renderBuffer.map);
    this.app.stage.addChild(this.parentComponent.renderBuffer.cursorStation);
    this.parentComponent.renderBuffer.buffer.forEach((sprite: Sprite, key: any, map: any) => {
      this.app.stage.addChild(sprite);
      sprite.tint = 111111;
      sprite.interactive = true;
      sprite.buttonMode = true;
      sprite.on("pointerdown", (e: any) => {
        this.parentComponent.spriteClicked(key);
      });
    });
  }

  CreateSprite(key: string) {
    let imageuri: string = <string>this.images.get(key);
    if (imageuri == null) throw "couldn't find image";
    return new Sprite(PIXI.loader.resources[imageuri].texture);
  }

  async LoadImages(image: string, key: string) {
    return new Promise(resolve => {
      try {
        PIXI.loader.add(image).load(() => {
          this.images.set(key, image);
          resolve()
        });
      } catch (er) {
        this.images.set(key, image);
        resolve();
      }
    });

  }

  Clear() {
    while (this.app.stage.children[0]) {
      this.app.stage.removeChild(this.app.stage.children[0]);
    }
  }


  async FixCanvas() {

    this.app.renderer.resize(this.width, this.height);
    this.app.renderer.autoResize = true;
    this.app.renderer.backgroundColor = 0xffffff;
    return;
  }
}

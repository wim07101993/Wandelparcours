import {Sprites} from "./Sprites";
import {Renderer} from "./Renderer";
import {RenderBuffer} from "./RenderBuffer";
import {ElementRef, ViewChild} from "@angular/core";
import {getBaseUrl} from "../app.module.browser";
import {MouseEvents} from "./MouseEvents";

export abstract class ARenderComponent {
    @ViewChild('myCanvas') canvasRef: ElementRef;
    renderer: Renderer;
    renderBuffer: RenderBuffer;
    adMarker: any;
    mouseEvents: MouseEvents;
    framerate = 60;
    zoomFactor: number = 1;
    markerscale=25;
    markersize=1;
    public async abstract LoadComponent(): Promise<boolean>;

    get BluePrintUrl() {
        return getBaseUrl() + "images/blueprint.jpg";
    }

    /**
     *      ngOnInit get called after the page is loaded
     *      Renderer, Renderbuffer and MouseEvent instances get instatieded
     *      images and the station locations get loaded, and the render buffer get cleaned and updated
     *      And an interval starts named tick
     */
    async ngOnInit() {
        //create renderer
        this.renderer = new Renderer(this);
        //set the imageurl
        this.renderBuffer = new RenderBuffer(this);
        this.mouseEvents = new MouseEvents(this);
        //load the blueprint of the building
        
        await this.LoadMap();
        await this.LoadComponent();
        await this.renderer.CleanAndUpdateRenderBuffer();
        //load marker
        setInterval(() => {
            this.Tick()
        }, 1000 / this.framerate);
    }

    async LoadMap() {

        //download the map
        await this.renderer.LoadImages(this.BluePrintUrl, Sprites.map);
        //put it in a sprite
        this.renderBuffer.map = await this.renderer.CreateSprite(Sprites.map);
        //adjust height and width
        this.renderBuffer.map.width = this.width;
        this.renderBuffer.map.height = this.height;

        return;


    }

    async Tick() {
        try {
            await this.renderer.FixCanvas();
            await this.RecalculateMap();
            
        } catch (ex) {
            console.log(ex);
        }
    }

    /*
    *    Getter calculates relative the width of the image
    */
    get width() {
        return this.renderer.width;
    }

    /*
    *    Getter calculates relative the height of the image
    */
    get height() {
        return this.renderer.height;
    }


    SaveStationToDatabaseModal(mouseposition: { x: number; y: number }) {

    }


    /*
     *   this function is needed to zoomin
     */
    async ZoomIn() {
        this.zoomFactor *= 2;
        await this.Tick();
    }

    /*
    *    this function is needed to zoomout
    */
    async ZoomOut() {

        this.zoomFactor /= 2;

        await this.Tick();
    }

    /*
    * Recalculates location and size of the image 
    */
    async RecalculateMap() {
        try {

            this.canvasRef.nativeElement.height = window.innerHeight;
            this.canvasRef.nativeElement.width = window.innerWidth;

            //calculate zoom
            let height = this.height * this.zoomFactor;
            let width = this.width * this.zoomFactor;
            //render
            let map = this.renderBuffer.map;
            map.width = width;
            map.height = height;
            map.x = this.mouseEvents.position.x;
            map.y = this.mouseEvents.position.y;
            this.mouseEvents.mapPos = {
                x: this.mouseEvents.position.x,
                y: this.mouseEvents.position.y,
                width: width,
                height: height
            };

        } catch (ex) {
            console.log(ex);
        }
    }
}
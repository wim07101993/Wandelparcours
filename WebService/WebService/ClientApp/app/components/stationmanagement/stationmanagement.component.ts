import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Http} from '@angular/http';
import {getBaseUrl} from "../../app.module.browser";
import {MouseEvents, Point} from "./MouseEvents"
import {Renderer} from "./Renderer"
import {RenderBuffer,} from "./RenderBuffer"
import {Station} from "../../models/station"
import {Sprites} from "./Sprites"
import {RestServiceService} from "../../service/rest-service.service"

declare var $: any;
declare var Materialize: any;

@Component({
    selector: 'app-stationmanagement',
    templateUrl: './stationmanagement.component.html',
    styleUrls: ['./stationmanagement.component.css']
})


/** Class representing stationmanagement page. */
export class StationmanagementComponent implements OnInit {
    @ViewChild('myCanvas') canvasRef: ElementRef;

    position: Point;
    renderBuffer: RenderBuffer;
    zoomFactor: number = 1;
    mouseEvents: MouseEvents;
    framerate = 5;
    imageUrl = "";
    markerUrl = "";
    adMarker: boolean = false;
    collidingElement: any;
    saveStation: Station = new Station();
    menu: boolean = false;
    stations = new Map<string, Point>();
    stationsIds = new Map<string, string>();
    stationMacAdresses: string[] = [];
    markerscale = 25;
    renderer: Renderer;
    markersize: number;
    rawstations:any;
    editing=false;
    editmac:string;
    /**
     * Creating stationmanagement page.
     * @param {RestServiceService} service  - A constructer injected service holding the service for rest connection
     */
    constructor(private service: RestServiceService) {
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
        this.imageUrl = getBaseUrl() + "images/blueprint.jpg";
        this.markerUrl = getBaseUrl() + "images/station.png";
        this.renderBuffer = new RenderBuffer(this);
        this.mouseEvents = new MouseEvents(this);
        //load the blueprint of the building
        await this.LoadMap();
        await this.DownloadMarker();
        await this.service.LoadStations(this);
        await this.renderer.CleanAndUpdateRenderBuffer();
        //load marker
        setInterval(() => {
            this.Tick()
        }, 1000 / this.framerate);
    }

    async UpdateStation(){
        try{
            
            let id = this.stationsIds.get(this.collidingElement);
            if (id==null|| id == undefined)
                throw "no el";
            let updateStatus= await this.service.UpdateStation(id, this.editmac);
            if (updateStatus==true){
                await this.service.LoadStations(this);
                this.CloseModal();
            }else{
                Materialize.toast('Er ging iets mis!', 4000);
            }
        }catch (e){
            
        }
    }
    /*
    *   Closes the modal to add a station 
    */
     async CloseModal() {
        
        $("#markerModel").modal("close");
    }

    /*
    *   Opens modal to delete a station 
    */
    async deleteModal(id?: string) {

        if (id != undefined) {
            this.collidingElement = id;
            this.editing = false;
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal();
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal("open");
        }


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
    *  tick does the needed calculatations for the render, and draws the rendering on the canvas
    */
    async Tick() {
        try {
            await this.renderer.FixCanvas();
            await this.RecalculateMap();
            await this.RecalculateStations();
            await this.DrawStationOnCursor();
        } catch (ex) {
            console.log(ex);
        }
    }

    /*
     *  This function causes to draw a station on the cursor when add station button is clicked 
     */
    async DrawStationOnCursor() {
        if (this.adMarker) {
            let width = this.width / this.markerscale;
            this.markersize = width;
            let x = this.mouseEvents.mousepos.x - (width / 2);
            let y = this.mouseEvents.mousepos.y - (width);
            let station = this.renderBuffer.cursorStation;
            station.x = x;
            station.y = y;
            station.width = width;
            station.height = width;
        } else {
            let station = this.renderBuffer.cursorStation;
            station.x = -9999999999999;
            station.y = -9999999999999;
            station.width = 0;
            station.height = 0;
        }
    }

    /*
    *   This function opens a modal to create a station 
    */
    async SaveStationToDatabaseModal(stationPosition: Point) {
        this.saveStation.position.x = stationPosition.x;
        this.saveStation.position.y = stationPosition.y;
        $("#markerModel").modal();
        $("#markerModel").modal("open");
    }

    /*
    *   this function loads the image of the building
    */
    async LoadMap() {
        //download the map
        await this.renderer.LoadImages(this.imageUrl, Sprites.map);
        //put it in a sprite
        this.renderBuffer.map = await this.renderer.CreateSprite(Sprites.map);
        //adjust height and width
        this.renderBuffer.map.width = this.width;
        this.renderBuffer.map.height = this.height;

        return;


    }

    /*
    *  load marker;
    */
    async DownloadMarker() {
        await this.renderer.LoadImages(this.markerUrl, "marker");
        this.renderBuffer.cursorStation = this.renderer.CreateSprite(Sprites.marker);
        this.renderBuffer.cursorStation.width = 0;
        this.renderBuffer.cursorStation.height = 0;
        this.renderBuffer.cursorStation.x = -99999;
        this.renderBuffer.cursorStation.y = -99999;
        console.log(this.renderBuffer.cursorStation);
        return;
    }

    /*
    *    Getter calculates relative the width of the image
    */
    get width() {
        let width = 1;
        let map = this.renderer.CreateSprite(Sprites.map);

        if (map == undefined) return 0;
        if (window.innerHeight > window.innerWidth) {
            width = window.innerHeight / map.height * map.width;
        } else {
            width = window.innerWidth;
        }
        return width;
    }

    /*
    *    Getter calculates relative the height of the image
    */
    get height() {
        let height = 0;
        let map = this.renderer.CreateSprite(Sprites.map);
        if (map == undefined) return 0;
        if (window.innerHeight > window.innerWidth) {
            height = window.innerHeight;

        } else {
            height = window.innerWidth / map.width * map.height;
        }
        return height;
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
            map.height = height ;
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

    /*
    * Recalculates location and size of all stations on the map 
    */
    async RecalculateStations() {
        let refreshNeeded = false;
        this.stations.forEach((location: Point, key: string, map: any) => {
            let width = this.width / this.markerscale;
            this.markersize = width;
            let position = this.mouseEvents.CalculateStationPosOnImage(location);
            let x = position.x - (width / 2);
            let y = position.y - width;
            let station = this.renderBuffer.buffer.get(key);
            if (station == undefined) {
                station = this.renderBuffer.AddSpriteToBufferById(key, Sprites.marker);
                refreshNeeded = true;
            }
            if (station != undefined) {
                station.x = x;
                station.y = y;
                station.width = width;
                station.height = width;
            }

        });
        if (refreshNeeded) {
            this.renderer.CleanAndUpdateRenderBuffer();
        }
    }


    /*
    *   This function will send request to the rest to delete station 
    */
    async DeleteCurrentStation() {
        await this.service.DeleteStation(this.collidingElement);
        await this.service.LoadStations(this);
        $("#deleteModal").modal("close");
    }

    /*
    *   This function will send request to the rest to save station 
    */
    async SaveNewStation() {
        let mac = this.saveStation.mac;
        let length = mac.length;
        if (length > 15 && length < 20 && mac.split(":").length==6) {
            await this.service.SaveStationToDatabase(this.saveStation);
            await this.service.LoadStations(this);
            this.saveStation = new Station();
            $("#markerModel").modal("close");
            this.adMarker = false;
        } else {
            Materialize.toast('Station adres verkeerd', 4000);
        }

    }

    ShowEditBox() {
        this.editmac= (' ' + this.collidingElement).slice(1);
        this.editing = !this.editing
    }
}

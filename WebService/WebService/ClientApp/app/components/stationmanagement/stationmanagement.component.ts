import {Component, OnInit} from '@angular/core';
import {Point} from "../../helpers/MouseEvents"
import {Station} from "../../models/station"
import {Sprites} from "../../helpers/Sprites"
import {RestServiceService} from "../../service/rest-service.service"
import {ARenderComponent} from "../../helpers/ARenderComponent"

declare var $: any;
declare var Materialize: any;

@Component({
    selector: 'app-stationmanagement',
    templateUrl: './stationmanagement.component.html',
    styleUrls: ['./stationmanagement.component.css']
})


/** Class representing stationmanagement page. */
export class StationmanagementComponent extends ARenderComponent implements OnInit {
    position: Point;
    imageUrl = "";
    markerUrl = "";
    collidingElement: any;
    saveStation: Station = new Station();
    menu: boolean = false;
    stations = new Map<string, Point>();
    stationMacAdresses: string[] = [];
    markerscale = 25;

    markersize: number;

    /**
     * Creating stationmanagement page.
     * @param {RestServiceService} service  - A constructer injected service holding the service for rest connection
     */
    constructor(private service: RestServiceService) {
        super();
    }


    async ngOnInit() {

        super.ngOnInit();

    }

    async Tick() {
        super.Tick();
        try {
            await this.RecalculateStations();
            await this.DrawStationOnCursor();
        } catch (ex) {
            console.log(ex);
        }
    }

    /*
    *   Closes the modal to add a station 
    */
    static async CloseModal() {
        $("#markerModel").modal("close");
    }

    /*
    *   Opens modal to delete a station 
    */
    async spriteClicked(id?: string) {

        if (id != undefined) {
            this.collidingElement = id;
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal();
            // noinspection JSJQueryEfficiency
            $("#deleteModal").modal("open");
        }


    }


    /*
    *  tick does the needed calculatations for the render, and draws the rendering on the canvas
    */


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
    *  load marker;
    */
    public async LoadComponent() {
        try {
        await this.renderer.LoadImages(this.markerUrl, "marker");
        this.renderBuffer.cursorStation = this.renderer.CreateSprite(Sprites.marker);
        this.renderBuffer.cursorStation.width = 0;
        this.renderBuffer.cursorStation.height = 0;
        this.renderBuffer.cursorStation.x = -99999;
        this.renderBuffer.cursorStation.y = -99999;
        console.log(this.renderBuffer.cursorStation);
            return true;
        } catch (e) {
            return false;
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
        let reg = new RegExp("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        let mac = this.saveStation.mac;
        if (reg.test(mac)) {

            await this.service.SaveStationToDatabase(this.saveStation);
            await this.service.LoadStations(this);
            this.saveStation = new Station();
            $("#markerModel").modal("close");
            this.adMarker = false;
        } else {
            Materialize.toast('Station adres verkeerd', 4000);
        }

    }
}

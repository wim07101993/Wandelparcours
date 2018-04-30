import {Component, OnInit, ElementRef} from '@angular/core';
import {Point} from "../../helpers/MouseEvents"
import {Station} from "../../models/station"
import {Sprites} from "../../helpers/Sprites"
import {RestServiceService} from "../../service/rest-service.service"
import {ARenderComponent} from "../../helpers/ARenderComponent"
import {getBaseUrl} from "../../app.module.browser";

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
  collidingElement: any;
  saveStation: Station = new Station();
  menu: boolean = false;
  stations = new Map<string, Point>();
  stationsIds = new Map<string, string>();
  stationMacAdresses: string[] = [];

  rawstations: any;
  editing = false;
  editmac: string;

  /**
   * Creating stationmanagement page.
   * @param {RestServiceService} service  - A constructer injected service holding the service for rest connection
   */
  constructor(private service: RestServiceService, protected elRef: ElementRef) {
    super();
    this.hostElement = this.elRef
  }

  get markerUrl() {
    return getBaseUrl() + "assets/images/station.png";
  }


  async ngOnInit() {
    super.ngOnInit();
    await setTimeout(async () => {
      await this.service.LoadStations(this);

    }, 100);

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
  //TODO
  async UpdateStation() {
    try {
      let id = this.stationsIds.get(this.collidingElement);
      if (id == null || id == undefined)
        throw "no el";
      let updateStatus = await this.service.UpdateStation(id, this.editmac);
      console.log(updateStatus);
      if (updateStatus == true) {
        await this.service.LoadStations(this);
        //this.CloseModal();
      } else {
        Materialize.toast('Er ging iets mis!', 4000);
      }
    } catch (e) {
        console.log('Errormessage: ' + e.toString());
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
  async spriteClicked(id?: string) {
    try {
      if (id != undefined) {
        this.collidingElement = id;
        this.editing = false;
        $("#deleteModal").modal();
        $("#deleteModal").modal("open");
      }
      return true;
    } catch (error) {
      return false
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
    let mac = this.saveStation.mac;
    let length = mac.length;
    if (length > 15 && length < 20 && mac.split(":").length == 6) {
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
    this.editmac = (' ' + this.collidingElement).slice(1);
    this.editing = !this.editing
  }
}

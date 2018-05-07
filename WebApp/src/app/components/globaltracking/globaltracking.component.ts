import {Component, OnInit, ElementRef} from '@angular/core';
import {Point} from "../../helpers/MouseEvents"
import {Station} from "../../models/station"
import {Sprites} from "../../helpers/Sprites"
import {RestServiceService} from "../../service/rest-service.service"
import {ARenderComponent} from "../../helpers/ARenderComponent"
import {getBaseUrl} from "../../app.module.browser";

declare var $: any;
declare var Materialize: any;

declare var $: any;

@Component({
  selector: 'app-globaltracking',
  templateUrl: './globaltracking.component.html',
  styleUrls: ['./globaltracking.component.css']
})
export class GlobaltrackingComponent extends ARenderComponent implements OnInit {

  position: Point;
  collidingElement: any;
  saveStation: Station = new Station();
  menu: boolean = false;
  stations = new Map<string, Point>();
  stationsIds = new Map<string, string>();
  stationMacAdresses: string[] = [];

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
    } catch (ex) {
      console.log(ex);
    }
  }



  /*
  *   Opens modal to delete a station
  */
  async spriteClicked(id?: string) {
    return true;
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
        //station = this.renderBuffer.AddSpriteToBufferById(key, Sprites.marker);
        station = this.renderBuffer.AddTextById(key);
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







}

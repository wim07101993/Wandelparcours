import {Component, ElementRef, OnInit} from '@angular/core';
import {Sprites} from '../../helpers/Sprites';
import {ARenderComponent} from '../../helpers/ARenderComponent';
import {Resident} from '../../models/resident';
import {getBaseUrl} from '../../app.module.browser';
import {ActivatedRoute, Router} from '@angular/router';
import {RestServiceService} from '../../service/rest-service.service';

declare var $: any;

@Component({
  selector: 'app-globaltracking',
  templateUrl: './globaltracking.component.html',
  styleUrls: ['./globaltracking.component.css']
})
export class GlobaltrackingComponent extends ARenderComponent implements OnInit {
  residents = new Map<string, Resident>();
  currentResident: Resident = new Resident();
  aResidents: Resident[] = new Array();
  id: string;

  async LoadComponent(): Promise<boolean> {
    try {

      await this.renderer.LoadImages(this.markerUrl, 'marker');
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
    //return undefined;
  }

  get markerUrl() {
    return getBaseUrl() + 'assets/images/resident.png';
  }

  constructor(private service: RestServiceService, protected elRef: ElementRef, private route: ActivatedRoute, private router: Router) {
    super();
    this.hostElement = elRef;

  }

  async ngOnInit() {
    await super.ngOnInit();
    this.id = this.route.snapshot.params['id'];
    this.residents = new Map<string, Resident>();
    await this.loadResidents();
    this.markerscale = 50;
    setInterval(() => {
      this.loadResidents();
    }, 5000);
  }

  async Tick() {
    super.Tick();
    await this.RecalculateResidents();


  }

  async spriteClicked(id?: string) {
    if (id == undefined)
      return false;
    try {
      let resident = this.residents.get(id);
      if (resident != undefined) {
        this.currentResident = resident;
        $('.modal').modal();
        $('#residentModal').modal('open');
      }
      return true;
    } catch (error) {
      return false;
    }
  }

  async loadResidents() {
    let loaded: any;

    if (this.id == undefined) {
      loaded = await this.service.getAllResidentsWithAKnownLastLocation();
    } else {
      loaded = await this.service.getOneResidentWithAKnownLastLocation(this.id);
      if (typeof loaded === 'boolean') {
        return;
  
        } else {
          loaded=[loaded];
        }
      }

    if (typeof loaded === 'boolean') {
      return;

    } else {
      try {
        let control: string[] = new Array();
        this.aResidents = [];
        loaded.forEach((resident: Resident) => {
          this.residents.set(resident.id, resident);
          this.aResidents.push(resident);
          control.push(resident.id);
        });
        let keyDeleted = false;
        this.residents.forEach((value, key, map) => {
          if (control.findIndex(i => i == key) == -1) {
            this.residents.delete(key);
            keyDeleted = true;
          }
        });


        if (keyDeleted) {
          this.renderBuffer.buffer.clear();
          await this.RecalculateResidents();
        }
      } catch (e) {
      }

    }
  }


  /*
  * Recalculates location and size of all residents on the map
  */
  async RecalculateResidents() {
    let refreshNeeded = false;
    this.residents.forEach((resident: Resident, key: string, map: any) => {
      let width = this.width / this.markerscale * this.zoomFactor;
      this.markersize = width;
      let position = this.mouseEvents.CalculateStationPosOnImage(resident.lastRecordedPosition);
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
  *   this function loads the image of the building
  */


  openListModal() {
    $('#modal1').modal();
    $('#modal1').modal('open');
    $('#modal1').css({'width': '25%', 'height': '100%'});
  }

  /*
 *   this function selects and navigates to personal tracking page of the resident
 */

  navigateToTracking(resident: Resident) {
    $('#modal1').modal('close');
    this.router.navigate([`/resident/${resident.id}/tracking`]);
  }

  /*
*   this function selects and navigates to perosnal page
*/

  navigateTo(resident: Resident) {
    $('#modal1').modal('close');
    this.router.navigate([`/resident/${resident.id}`]);
  }

}

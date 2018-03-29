import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Renderer} from "../../helpers/Renderer"
import {RenderBuffer,} from "../../helpers/RenderBuffer"
import {Station} from "../../models/station"
import {Sprites} from "../../helpers/Sprites"
import {ARenderComponent} from "../../helpers/ARenderComponent"
import {getBaseUrl} from "../../app.module.browser";
import { Http } from '@angular/http';
import { Resident } from "../../models/resident";
@Component({
  selector: 'app-globaltracking',
  templateUrl: './globaltracking.component.html',
  styleUrls: ['./globaltracking.component.css']
})
export class GlobaltrackingComponent extends  ARenderComponent  implements OnInit {
    residents=new Map<string,Resident>();

    async LoadComponent(): Promise<boolean> {
        try{
            await this.renderer.LoadImages(this.markerUrl, "marker");
            this.renderBuffer.cursorStation = this.renderer.CreateSprite(Sprites.marker);
            this.renderBuffer.cursorStation.width = 0;
            this.renderBuffer.cursorStation.height = 0;
            this.renderBuffer.cursorStation.x = -99999;
            this.renderBuffer.cursorStation.y = -99999;
            console.log(this.renderBuffer.cursorStation);
            return true;
        }catch (e){
            return false;
        }
      //return undefined;
    }

    get markerUrl(){
        return getBaseUrl() + "images/resident.png";
    }
    
  constructor(private http:Http) {
      super();
  }

  async ngOnInit() {
        super.ngOnInit();
        this.loadResidents();
        this.markerscale=50;
  }
  
  async Tick(){
        super.Tick();
        await this.RecalculateResidents();
        await this.loadResidents();
  }

  async spriteClicked(){
      //alert("clicked");
  }

    async loadResidents(){
           let loaded= await new Promise<any>(resolve=>{
                try{
                    this.http.get(getBaseUrl()+"api/v1/location/residents/lastlocation").subscribe(response=>{
                        resolve(response.json());
                    },error=>{
                        resolve(false);
                    });
                }catch(e){resolve(false)}   
           });
           if(typeof loaded==="boolean"){
               //TODO show material notify
            
           }else{
            loaded.forEach((resident:Resident)=>{
                this.residents.set(resident.id,resident);
            });

           }
    }



    /*
    * Recalculates location and size of all residents on the map 
    */
   async RecalculateResidents() {
    let refreshNeeded = false;
    this.residents.forEach((resident: Resident, key: string, map: any) => {
        let width = this.width / this.markerscale;
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
  
}

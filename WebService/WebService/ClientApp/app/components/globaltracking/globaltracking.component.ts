import { Component, ElementRef, OnInit, ViewChild, Input } from '@angular/core';
import { Renderer } from "../../helpers/Renderer"
import { RenderBuffer, } from "../../helpers/RenderBuffer"
import { Station } from "../../models/station"
import { Sprites } from "../../helpers/Sprites"
import { ARenderComponent } from "../../helpers/ARenderComponent"
import { Http } from '@angular/http';
import { Resident } from "../../models/resident";
import { getBaseUrl } from "../../app.module.browser";
import {ActivatedRoute} from '@angular/router';
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
        //return undefined;
    }

    get markerUrl() {
        return getBaseUrl() + "images/resident.png";
    }

    constructor(private http: Http, protected elRef: ElementRef,private route: ActivatedRoute) {
        super();
        this.hostElement = elRef;

    }

    async ngOnInit() {
        super.ngOnInit();
        this.id = this.route.snapshot.params['id'];
        this.residents=new Map<string,Resident>();
        this.loadResidents();
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
            return false
        try {
            let resident = this.residents.get(id);
            if (resident != undefined) {
                this.currentResident = resident;
                $('.modal').modal();
                $('#residentModal').modal("open");
            }
            return true;
        } catch (error) {
            return false;
        }
        //alert("clicked");
    }

    async loadResidents() {
        let loaded: any;
        
        if (this.id == undefined) {
            loaded =await new Promise<any>(resolve => {
                try {
                    this.http.get(getBaseUrl() + "api/v1/location/residents/lastlocation").subscribe(response => {

                        resolve(response.json());
                    }, error => {
                        resolve(false);
                    });
                } catch (e) { resolve(false) }
            });

        }else{
            loaded= await new Promise<any>(resolve=>{
                try {
                    this.http.get(getBaseUrl() + "api/v1/location/residents/"+this.id+"/lastlocation").subscribe(response => {
                        resolve([response.json()]);
                    }, error => {
                        resolve(false);
                    });
                } catch (e) { resolve(false) }
            });
            
        }

        if (typeof loaded === "boolean") {
            return;

        } else {
            try{
                let control:string[]=new Array();
                this.aResidents = [];
                loaded.forEach((resident: Resident) => {
                    this.residents.set(resident.id, resident);
                    this.aResidents.push(resident);
                    control.push(resident.id);
                });
                let keyDeleted=false;
                this.residents.forEach((value,key,map) =>{
                    if(control.findIndex(i => i == key)==-1){
                        this.residents.delete(key);
                        keyDeleted=true;
                    }
                })
                    
                
                
                    
                if(keyDeleted){
                    this.renderBuffer.buffer.clear();
                    this.RecalculateResidents();
                }
            }catch(e){}

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

}

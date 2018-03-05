
import {StationmanagementComponent} from "./stationmanagement.component"
import {Point} from "./MouseEvents"
export class RenderBuffer{
    station:StationmanagementComponent;
    buffer:bufferelement[];
    
    constructor(station:StationmanagementComponent){
        this.station=station;
        this.buffer=[];
    }
    
    async render() {
        return new Promise(resolve => {
            
            for ( let element of this.buffer){
                this.station.context.drawImage(element.image,element.x, element.y, element.width,element.height);
            }
        });
        

    }
    
    async getElementById(id:string){
        let be=null;
        for (be of this.buffer){
            if (be.id==id){
                
                return be;
                
            }
        }
    }
    
    async addAnonym(image:HTMLImageElement,x:number,y:number,width:number,heigth:number){
        return new Promise(resolve => {
            let element:bufferelement={image:image,x:x, y:y ,width:width,height:heigth,id:"",className:""};
            this.buffer.push(element);
            resolve();
        });
    }
    async add(image:HTMLImageElement,x:number,y:number,width:number,heigth:number,id:string,className:string ){
        return new Promise(resolve => {
            let element:bufferelement={image:image,x:x, y:y ,width:width,height:heigth,id:id,className:className};
            this.buffer.push(element);
            resolve();
        });
    }
    async clear(){
        return new Promise(resolve => {
            this.buffer=[];
            
            this.station.context.clearRect(0,0,this.station.canvas.width,this.station.canvas.height);
            resolve();
        })
    }
}
export interface bufferelement{
    image:HTMLImageElement;
    x:number; 
    y:number;
    width:number;
    height:number;
    id:string;
    className:string;
    
}
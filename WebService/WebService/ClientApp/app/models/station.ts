export class Station{
    id:string;
    isActive:boolean;
    mac:string;
    position:Position;
    
    constructor(){
        this.id="";
        this.isActive=false;
        this.mac="";
        this.position=new Position();
    }
    
}

export class Position{
    timeStamp:string;
    x:number;
    y:number;
    constructor(){
        this.timeStamp="";
        this.x=0;
        this.y=0;
    }
}
class AbstractViewer{
    constructor(){
        var Store = require("electron-store");
        this.http = require("axios");
        this.scanClosest= require(__dirname+"/scanClosest").scanClosest;
        this.store = new Store();
        this.url = this.store.get("resturl");
        this.scanBeacon();
        this.debugTimeout=0;
    }

    loadData(){}

    scanBeacon(){
        this.scanClosest().then((beacon)=>{
                this.beacon=beacon;
                this.loadData();
            
        });
    }

    timeOut(delay){
        
        this.debugTimeout++;
        var deepcopy = this.debugTimeout+"";

        console.log(`timeout start ${deepcopy}`)
        return new Promise((resolve)=>{
            var timeEnd = Date.now() + delay;
            var interval = setInterval(()=>{
                var dif = Date.now()- timeEnd;
                if(dif > 0)
                {
                    console.log(`timeout stop  ${deepcopy}`)
                    clearInterval(interval);
                    resolve();
                }
            },10);
            //while (Date.now() < before + delay){}
              //  resolve();
                
            
            
        })
    }

}

exports.AbstractViewer = AbstractViewer;
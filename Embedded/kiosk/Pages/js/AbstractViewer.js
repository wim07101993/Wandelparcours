class AbstractViewer{
    constructor(){
        var Store = require("electron-store");
        this.http = require("axios");
        this.scanClosest= require(__dirname+"/scanClosest").scanClosest;
        this.store = new Store();
        this.url = this.store.get("resturl");
        this.debugTimeout=0;
        this.username='module';
        this.password="kiosktoermalien"
        this.token=null;
        this.login();
        this.scanBeacon();
    }
    refreshToken(){
        const http = this.http.create({
          headers: {'userName': this.username,"password":this.password}
        });
        this.http.post(`${this.url}/api/v1/tokens`).then((result)=>{
          this.token=result.data.token;
          this.level=result.data.user.userType;
        }).catch(()=>{
          setTimeout(()=>{
            this.refreshToken();
          },60*1000);
        });
          
      }
     axios(){
        const instance = this.http.create({
          headers: {'token': this.token,'Content-type' : 'application/json'}
        });
        return instance;
      }
    login(){
        
        const http = this.http.create({
          headers: {'userName': this.username,"password":this.password}
        });
        try{
           http.post(`${this.url}/api/v1/tokens`).then((token)=>{
              this.token = token.data.token;
              this.level= token.data.user.userType;
              this.refreshTokenInterval =setInterval(()=>{this.refreshToken()},10*60*1000);
          });
        }catch(ex){
            setTimeout(()=>{this.login()},2000);
        }
    
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
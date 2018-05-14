
class AbstractViewer{
    constructor(){
        var Store = require("electron-store");
        this.http = require("axios");
        this.scanClosest= require(__dirname+"/scanClosest").scanClosest;
        this.store = new Store();
        this.url = this.store.get("resturl");
        this.debugTimeout=0;
        this.username='Modul3';
        this.password="KioskTo3rmali3n"
        this.token=null;
        this.login();
        this.scanBeacon();
    }
    /**
   * log in every 10 min to refresh the token 
   */
    refreshToken(){
        const http = this.http.create({
          headers: {'userName': this.username,"password":this.password}
        });
        http.post(`${this.url}/api/v1/tokens`).then((result)=>{
          this.token=result.data.token;
          this.level=result.data.user.userType;
        }).catch(()=>{
          setTimeout(()=>{
            this.refreshToken();
          },10*60*1000);
        });
          
      }
      /**
       * returns an axios instance with a valid token
       */
     axios(){
        const instance = this.http.create({
          headers: {'token': this.token,'Content-type' : 'application/json'}
        });
        return instance;
      }
      axiosBlob(){
        const instance = this.http.create({   
          responseType: 'blob',
          headers: {'token': this.token,'Content-type' : 'application/json'}
        });
        return instance;
      }
      /**
       * logs in the station in the system
       */
    login(){
        
        const http = this.http.create({
          headers: {'userName': this.username,"password":this.password}
        });
        try{
           http.post(`${this.url}/api/v1/tokens`).then((token)=>{
              this.token = token.data.token;
              this.level= token.data.user.userType;
              this.refreshTokenInterval =setInterval(()=>{this.refreshToken()},10*60*1000);
          }).catch((e)=>{
            setTimeout(()=>{this.login()},2000);
          });
        }catch(ex){
            setTimeout(()=>{this.login()},2000);
        }
    
      }
    loadData(){}
      /**
       * this function loads and returns the closest beacon
       */
    scanBeacon(){
        this.scanClosest().then((beacon)=>{
                this.beacon=beacon;
                this.loadData();
            
        });
    }
    /**
     * this is a promise based timer
     * @param {*} delay how much you want to delay
     */
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
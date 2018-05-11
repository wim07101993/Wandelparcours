var AbstractViewer = require(__dirname+"/js/AbstractViewer").AbstractViewer;
class AudioObject extends AbstractViewer{
    constructor(){
        super();
    }
    
    loadData(){
        if(this.beacon==null|| this.beacon == undefined){
            this.scanBeacon();
            return;
        }else{
            this.axios().get(`${this.url}/API/v1/residents/bytag/${this.beacon}/music/random`).then((request) => {
                try {
                    var id = request.data.id;
                    let url= `${this.url}/api/v1/media/${id}/file.${request.data.extension}?token=${this.token}`;
                    this.axiosBlob().get(url).then((r)=>{
                        let audioBlob=r.data;
                        var audiofile = URL.createObjectURL(audioBlob); // IE10+
                        var audio = document.createElement('audio');
                        audio.controls="controls"
                        audio.preload="auto";
                        audio.src =audiofile;
                        audio.autoplay = true;
                        $("#audio").empty()
                        $(audio).appendTo("#audio");
                        audio.onended=()=>{
                            $("#audio").empty()
                            this.scanBeacon()};    
                        
                    }).catch((e)=>{this.scanBeacon();});

                } catch (error) {
                    this.scanBeacon();
                }
            }).catch(()=>{
                setTimeout(() => {
                    this.scanBeacon();
                }, 5000);
            });
        }
    }
    
}

var audio = new AudioObject();
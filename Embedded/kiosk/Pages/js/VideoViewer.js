var AbstractViewer =  require(__dirname+"/js/AbstractViewer").AbstractViewer;


class VideoViewer extends AbstractViewer{
    constructor(){
        super();
    }
    
    loadData(){
        if(this.beacon==null|| this.beacon == undefined){
            this.scanBeacon();
            return;
        }else{
            this.http.get(`${this.url}/API/v1/residents/bytag/${this.beacon}/videos/random`).then((request) => {
                try {
                    var id = request.data.id;
                    var video = document.createElement('video');
                    video.src = `${this.url}/api/v1/media/${id}/file`;
                    video.autoplay = true;
                    $("#video").empty()
                    $(video).appendTo("#video");
                    if ($(video).height() > $(video).width()) {
                        $(video).css({ 'height': '100%' });
                    } else {
                        $(video).css({ 'width': '100%' });
                    }
                    video.onended=()=>{
                        $("#video").empty()
                        this.scanBeacon()};    
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

var videoViewer = new VideoViewer();
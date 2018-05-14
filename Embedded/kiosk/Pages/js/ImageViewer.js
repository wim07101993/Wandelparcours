
var AbstractViewer = require(__dirname+"/js/AbstractViewer").AbstractViewer;
class ImageViewer extends AbstractViewer{
    constructor(){
        
        super();
        this.images = [];
        this.index = 0;
        //in sec
        this.reloadBeaconsSpeed=60;
        //in sec
        this.loopspeed = 6;

    }
    /**
     * loads the images for the tag
     */
    loadData(){
        
        if(this.beacon==null|| this.beacon == undefined){
            this.scanBeacon();
            return;
        }else{
            console.log("beacon loaded");
            this.axios().get(`${this.url}/API/v1/residents/bytag/${this.beacon}/images`).then((request) => {
                console.log("request done");
                this.images = request.data;
                this.images=this.shuffle(this.images);
                setTimeout(()=>{
                    try{

                        this.index=0;
                        this.images=[];
                        this.nextLoop.cancel();
                    }catch(e){console.log(e)}
                    
                    this.scanBeacon();
                },this.reloadBeaconsSpeed * 1000);
                this.createLoop();

                if (this.images.length == undefined || this.images.length == 0 || this.images.length == null) {
                    this.scanBeacon();
                }
            }).catch(()=>{
                console.log("couldn't request");
                setTimeout(() => {
                    this.scanBeacon();
                }, 5000);
            });
        }
    }
    /**
     * creates a carousel for the images
     */
    createLoop() {
        var img = new Image()
        img.onload = ()=> { 
           this.nextLoop=this.timeOut((this.loopspeed*1000));
            this.nextLoop.then(()=>{this.createLoop()});
            this.fixSize(img)
        };
        img.src = `${this.url}/api/v1/media/${this.images[this.index].id}/file?token=${this.token}`;
        this.index++;

        if (this.index > this.images.length - 1) {
            this.index = 0;
        }
        

    }
    /**
     * fixes the aspect ratio for full screen for image
     * @param {*} img source image
     */
    fixSize(img) {
        $("#images").fadeOut(500,()=>{
            $("#images").empty();
            $(img).appendTo("#images");
            if(img.height==img.width){
                if(window.innerHeight>window.innerWidth){
                    $(img).css({ 'width': '100%' });
                }else{
                    $(img).css({ 'height': '100%' });
                }
            }else{
                if (img.height > img.width) {
                    $(img).css({ 'height': '100%' });
                } else {
                    $(img).css({ 'width': '100%' });
                }
            }
            $("#images").fadeIn(500);
        });
        
    }
    /**
     * shuffle's the list of images
     */
    shuffle(a) {
        for (var i = a.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [a[i], a[j]] = [a[j], a[i]];
        }
        return a;
    }
}

var imageViewer = new ImageViewer();
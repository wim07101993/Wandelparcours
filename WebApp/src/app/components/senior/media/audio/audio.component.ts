import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {LoginService} from '../../../../service/login-service.service';
import {MediaService} from '../../../../service/media.service';
import {Resident} from '../../../../models/resident';
declare var $: any;

@Component({
  selector: 'app-audio',
  templateUrl: './audio.component.html',
  styleUrls: ['./audio.component.css']
})
export class AudioComponent implements OnInit {
  typeOfMedia: string;
    fullLinks: any=[];
    id: string = this.route.snapshot.params['id'];
    music: string = "/music";
    deleteResidentMusic: Resident;
  constructor(private route: ActivatedRoute, private media: MediaService,private login:LoginService) { }

  ngOnInit() {
      this.getAllMusic();
      this.typeOfMedia = "audio/*";
      this.deleteResidentMusic = <Resident>{music: {}};
  }

    reload() {
        this.getAllMusic();
    }

    async getAllMusic(){
        this.fullLinks = [];
        this.fullLinks = await this.media.getMedia(this.id, this.music);
        console.log(this.fullLinks);
    }

    async deleteResidentMediaByUniqueId(uniqueAudioID: string) {
        await this.media.deleteMedia(this.id, uniqueAudioID, this.music);
        setTimeout(()=>{
            $("#deleteModal").modal("close");
        }, 200)
        this.getAllMusic();
    }

    CloseModal() {
        $("#deleteModal").modal("close");
    }

    /*
    *   Opens modal to delete a picture
    */
    deleteModal(resident: Resident) {
        this.deleteResidentMusic = resident;
        $("#deleteModal").modal();
        $("#deleteModal").modal("open");
    }
}

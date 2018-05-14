import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {LoginService} from '../../../../service/login-service.service';
import {MediaService} from '../../../../service/media.service';
import {Resident} from '../../../../models/resident';
declare var $: any;
declare var Materialize: any;
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

    /**
     * Injectable params in constructor
     * @param {ActivatedRoute} route
     * @param {MediaService} media
     * @param {LoginService} login
     */
  constructor(private route: ActivatedRoute, private media: MediaService,private login:LoginService) { }

  ngOnInit() {
      this.getAllMusic();
      this.typeOfMedia = "audio/*";
      this.deleteResidentMusic = <Resident>{music: {}};
  }

    /**
     * Reload page after adding a audio file
     */
    reload() {
        this.getAllMusic();
    }

    /**
     * Get all URLS for music --> music will not be displayed only titles etc
     * for further implementation see thesis for future references
     * @returns {Promise<void>}
     */
    async getAllMusic(){
        this.fullLinks = [];
        this.fullLinks = await this.media.getMedia(this.id, this.music);
    }

    /**
     * Delete the media of the resident based on ID of media
     * @param {string} uniqueAudioID
     * @returns {Promise<void>}
     */
    async deleteResidentMediaByUniqueId(uniqueAudioID: string) {
        await this.media.deleteMedia(this.id, uniqueAudioID, this.music);
        Materialize.toast('Media succesvol verwijderd!',5000);
        setTimeout(()=>{
            $("#deleteModal").modal("close");
        }, 200)
        this.getAllMusic();
    }

    /**
     * Closes modal
     * @constructor
     */
    CloseModal() {
        $("#deleteModal").modal("close");
    }

    /**
     * Delete Audio with modal popup
     * @param {Resident} resident
     */
    deleteModal(resident: Resident) {
        this.deleteResidentMusic = resident;
        $("#deleteModal").modal();
        $("#deleteModal").modal("open");
    }

    /**
     * Opens modal this doesnt do anything just yet
     * @param modalResident
     */
    openModal(modalResident: Resident) {
        this.deleteResidentMusic = modalResident;
        $('#deleteModalResident').modal();
        $('#deleteModalResident').modal('open');
    }
}

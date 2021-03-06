import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Resident} from '../../../../models/resident';
import {MediaService} from '../../../../service/media.service';
import { LoginService } from '../../../../service/login-service.service';
declare var $: any;
declare var Materialize: any;
@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.css'],
})
export class VideoComponent implements OnInit {
  typeOfMedia: string;
  video: string = "/videos";
  id: string = this.route.snapshot.params['id'];
  check: any;

  deleteResidentVideo: Resident;
  videos: Resident[];
  fullLinks: any = [];
  url: any = "http://localhost:5000/api/v1/media/";

  constructor(private route: ActivatedRoute, private media: MediaService,private login:LoginService) {}

  ngOnInit() {
      this.typeOfMedia = "video/*";
      this.getAllVideos();
      this.deleteResidentVideo = <Resident>{videos: {}}
    }
  /**
   * reload the page
   */
  reload() {
    this.getAllVideos();
  }

  /**
     * Get all the URLS for videos
     * @returns {Promise<void>}
     */
  async getAllVideos() {
    this.fullLinks = [];
    this.fullLinks = await this.media.getMedia(this.id, this.video);
  }

  /**
     *  Delete resident media based on uniqueID
     * @param {string} uniqueVideoId  unique videoId
     * @returns {Promise<void>} reloads the page or sends user
     */
  async deleteResidentMediaByUniqueId(uniqueVideoId: string) {
      await this.media.deleteMedia(this.id, uniqueVideoId, this.video);
      Materialize.toast('Media succesvol verwijderd!',5000);
      setTimeout(() => {
          $("#deleteModalVideo").modal("close");
      }, 200)
      this.getAllVideos();
  }

  /**
     * Closes modal
     * @returns {Promise<void>}
     * @constructor
     */
  async CloseModal() {
    $("#deleteModalVideo").modal("close");
  }

  /**
     * Open delete modal
     * @param {Resident} resident
     * @returns {Promise<void>}
     */
  async deleteModal(resident: Resident) {
    this.deleteResidentVideo = resident;
    $("#deleteModalVideo").modal();
    $("#deleteModalVideo").modal("open");
  }
}

import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Resident} from '../../../../models/resident';
import {MediaService} from '../../../../service/media.service';

declare var $: any;

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

  constructor(private route: ActivatedRoute, private media: MediaService) {}


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
   * gets all urls for videos
   */
  async getAllVideos() {
    this.fullLinks = [];
    this.fullLinks = await this.media.getMedia(this.id, this.video);
    console.log(this.fullLinks)
  }

  /**
   * Delete resident media based on uniqueID
   * @param uniqueVideoId unique videoId
   * Either reloads the page or sends user to error page
   */
  async deleteResidentMediaByUniqueId(uniqueVideoId: string) {
      await this.media.deleteMedia(this.id, uniqueVideoId, this.video);
      setTimeout(() => {
          $("#deleteModalVideo").modal("close");
      }, 200)
      this.getAllVideos();
  }

  /*
  *   Closes the modal to add a station
  */
  async CloseModal() {
    $("#deleteModalVideo").modal("close");
  }

  /*
  *   Opens modal to delete a station
  */
  async deleteModal(resident: Resident) {
    this.deleteResidentVideo = resident;
    $("#deleteModalVideo").modal();
    $("#deleteModalVideo").modal("open");
  }
}

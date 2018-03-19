import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestServiceService } from '../../../../service/rest-service.service';
import { Resident } from '../../../../models/resident';
import { MediaService } from '../../../../service/media.service';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.css'],
})
export class VideoComponent implements OnInit {
    typeOfMedia: string;
    video: string = "/videos"
    id: string = this.route.snapshot.params['id'];
    check: any;

    videos: Resident[];
    fullLinks: any = [];
    url: any = "http://localhost:5000/api/v1/media/";

    constructor(private route: ActivatedRoute, private media: MediaService, private router: Router) {
        this.typeOfMedia = "video/*";
        this.getAllVideos();
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
        this.check = await this.media.deleteMedia(this.id, uniqueVideoId, this.video);
        if (this.check) {
            this.getAllVideos();
        } else {
            this.router.navigate(["/error"]);
        }
    }

    ngOnInit() {
    }

}

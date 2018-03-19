import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RestServiceService } from '../../../../service/rest-service.service';
import { Resident } from '../../../../models/resident';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.css'],
})
export class VideoComponent implements OnInit {
    //http://videogular.github.io/videogular2/docs/getting-started/how-videogular-works.html
    typeOfMedia: string;
    id: string = this.route.snapshot.params['id'];

    videos: Resident[];
    fullLinks: any = [];
    url: any = "http://localhost:5000/api/v1/media/";

    constructor(private route: ActivatedRoute, private service: RestServiceService) {
        this.typeOfMedia = "video/*";
        this.getAllVideos();
    }

    reload() {
        this.getAllVideos();
    }

    async getAllVideos() {
        this.fullLinks = [];
        this.videos = await this.service.getVideosOfResidentBasedOnId(this.id);
        //console.log(this.videos);

        for (let a of this.videos) {
            let url2 = this.url + a.id;
            let fullLinks = new Resident();

            fullLinks.videos.id = a.id;
            fullLinks.videos.url = url2;
            //fullLinks.videos.name = a.videos.name;
            //console.log(fullLinks);
            this.fullLinks.push(fullLinks);
        }
        //this.fullLinks;
        console.log(this.fullLinks);
    }

    async deleteVideoOnId(uniqueVideoId: string) {
        //console.log("deleted");
        let check = await this.service.deleteResidentVideoByUniqueId(this.id, uniqueVideoId);
        if (check) {
            this.getAllVideos();
        }
        //this.getAllVideos();
    }

  ngOnInit() {
  }

}

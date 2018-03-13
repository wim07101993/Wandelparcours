import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-video',
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.css']
})
export class VideoComponent implements OnInit {
    typeOfMedia: string;

    constructor() {
        this.typeOfMedia = "video/*";
    }

  ngOnInit() {
  }

}
